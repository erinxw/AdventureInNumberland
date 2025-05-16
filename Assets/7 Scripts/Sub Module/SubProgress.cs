using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class SubProgress : MonoBehaviour
{
    public Slider progressBar;
    public GameObject progressBarHighlight;
    public int totalActivities = 5;
    public string sceneName;

    private int completedActivities = 0;
    private bool isTapped = false;
    private DatabaseReference dbReference;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (progressBarHighlight != null) progressBarHighlight.SetActive(false);

        LoadProgressFromFirebase();
    }

    void Update()
    {
        if (Touchscreen.current == null || isTapped) return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject tapped = hit.collider.gameObject;

                if (tapped.CompareTag("CorrectAnswer"))
                {
                    isTapped = true;
                    completedActivities++;

                    UpdateProgressBar();
                    SaveProgressToFirebase();
                    SceneManager.LoadScene(sceneName);
                }
            }
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = totalActivities;
            progressBar.SetValueWithoutNotify(completedActivities);
        }

        if (progressBarHighlight != null)
        {
            progressBarHighlight.SetActive(true);
            Invoke(nameof(HideHighlight), 1.0f);
        }

        Debug.Log("Sub Progress bar updated: " + completedActivities + "/" + totalActivities);
    }

    private void HideHighlight()
    {
        if (progressBarHighlight != null)
            progressBarHighlight.SetActive(false);
    }

    private void SaveProgressToFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("User not logged in. Cannot save progress.");
            return;
        }

        dbReference.Child("users").Child(userId).Child("subProgress").SetValueAsync(completedActivities)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save sub progress: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    PlayerPrefs.SetInt("subProgress", completedActivities);
                    Debug.Log("Sub progress saved to Firebase: " + completedActivities);
                }
            });
    }

    private void LoadProgressFromFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("User not logged in. Cannot load progress.");
            completedActivities = PlayerPrefs.GetInt("subProgress", 0); // fallback
            UpdateProgressBar();
            if (progressBar != null) progressBar.gameObject.SetActive(true);
            return;
        }

        dbReference.Child("users").Child(userId).Child("subProgress").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to load sub progress: " + task.Exception);
                    completedActivities = PlayerPrefs.GetInt("subProgress", 0); // fallback
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out int savedProgress))
                    {
                        completedActivities = savedProgress;
                        Debug.Log("Loaded sub progress from Firebase: " + completedActivities);
                    }
                    else
                    {
                        completedActivities = 0;
                        Debug.Log("No saved sub progress found, starting at 0.");
                    }
                }

                UpdateProgressBar();
                if (progressBar != null) progressBar.gameObject.SetActive(true);
            });
    }
}
