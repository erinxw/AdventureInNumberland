using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class AddProgress : MonoBehaviour
{
    private DatabaseReference dbReference;

    public Slider progressBar;
    public GameObject progressBarHighlight;
    public int totalActivities = 5;

    public AudioSource audioSource;        
    public AudioClip clickSound;           

    public string nextSceneName;           

    private int completedActivities;
    private bool isAudioPlaying = false;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        completedActivities = PlayerPrefs.GetInt("addProgress", 0);
        UpdateProgressBar();
        LoadProgress();
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleInput(touchPosition);
        }
    }

    void HandleInput(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            OnClick();
        }
    }

    void OnClick()
    {
        UpdateCompletedActivity();

        if (isAudioPlaying || clickSound == null || audioSource == null) return;

        isAudioPlaying = true;
        audioSource.PlayOneShot(clickSound);
        Debug.Log("Correct answer clicked!");

        // Reset audio flag after sound finishes
        Invoke(nameof(ResetAudioFlag), clickSound.length);

        // Optional: Load next scene after sound
        if (!string.IsNullOrEmpty(nextSceneName))
            Invoke(nameof(LoadNextScene), clickSound.length);
    }

    void ResetAudioFlag()
    {
        isAudioPlaying = false;
    }

    void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private void UpdateCompletedActivity()
    {
        if (completedActivities < totalActivities)
        {
            completedActivities++;
            UpdateProgressBar();
            SaveProgressToDatabase();
        }
    }

    private void LoadProgress()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated. Cannot load progress.");
            return;
        }

        string userId = user.UserId;

        dbReference.Child("users").Child(userId).Child("progress").Child("addProgress").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    completedActivities = Convert.ToInt32(task.Result.Value);
                    UpdateProgressBar();
                    PlayerPrefs.SetInt("addProgress", completedActivities);
                }
            });
    }

    private void SaveProgressToDatabase()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated. Cannot save progress.");
            return;
        }

        string userId = user.UserId;

        dbReference.Child("users").Child(userId).Child("progress").Child("addProgress").SetValueAsync(completedActivities)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    PlayerPrefs.SetInt("addProgress", completedActivities);
                    Debug.Log($"Progress saved: addProgress = {completedActivities}");
                }
            });
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
    }

    private void HideHighlight()
    {
        if (progressBarHighlight != null)
            progressBarHighlight.SetActive(false);
    }
}
