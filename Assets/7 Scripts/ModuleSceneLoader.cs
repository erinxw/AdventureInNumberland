using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public enum ModuleType { Count, Add, Sub }

public class ModuleSceneLoader : MonoBehaviour
{
    public ModuleType moduleType;  // Set in inspector or dynamically

    public GameObject resetPopup;

    private DatabaseReference dbReference;
    private string progressKey;

    void Awake()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        if (resetPopup != null)
        {
            resetPopup.SetActive(false);
        }

        // Set progress key based on module type
        switch (moduleType)
        {
            case ModuleType.Count:
                progressKey = "countProgress";
                break;
            case ModuleType.Add:
                progressKey = "addProgress";
                break;
            case ModuleType.Sub:
                progressKey = "subProgress";
                break;
        }
    }

    public void LoadModule()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not authenticated.");
            return;
        }

        string userId = user.UserId;
        dbReference.Child("users").Child(userId).Child("progress").Child(progressKey).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    int progress = 0;

                    if (task.Result.Exists)
                    {
                        progress = int.Parse(task.Result.Value.ToString());
                        Debug.Log("Loaded progress: " + progress);
                    }

                    if (progress >= 5)
                    {
                        if (resetPopup != null)
                        {
                            resetPopup.SetActive(true);
                        }
                        return;  // Wait for user action on popup
                    }

                    string sceneToLoad = GetSceneNameFromProgress(progress);
                    Debug.Log("Loading scene: " + sceneToLoad);
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogError("Error retrieving progress: " + task.Exception);
                }
            });
    }

    private string GetSceneNameFromProgress(int stage)
    {
        // You can customize scene names for each module type
        switch (moduleType)
        {
            case ModuleType.Count:
                return stage switch
                {
                    0 => "CountingTutorial",
                    1 => "CountingTwo",
                    2 => "CountingThree",
                    3 => "CountingFour",
                    4 => "CountingFive",
                    _ => "CountingTutorial"
                };
            case ModuleType.Add:
                return stage switch
                {
                    0 => "AddTutorial",
                    1 => "Add2",
                    2 => "Add3",
                    3 => "Add4",
                    4 => "Add5",
                    _ => "AddTutorial"
                };
            case ModuleType.Sub:
                return stage switch
                {
                    0 => "SubTutorial",
                    1 => "Sub2",
                    2 => "Sub3",
                    3 => "Sub4",
                    4 => "Sub5",
                    _ => "SubTutorial"
                };
            default:
                return "CountingTutorial";
        }
    }

    public void ResetProgress()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("[ModuleSceneLoader] Cannot reset progress. User not authenticated.");
            return;
        }

        string userId = user.UserId;
        Debug.Log("[ModuleSceneLoader] Resetting progress for user: " + userId + ", key: " + progressKey);

        dbReference.Child("users").Child(userId).Child("progress").Child(progressKey).SetValueAsync(0)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("[ModuleSceneLoader] Progress reset successfully. Reloading tutorial scene.");
                    string sceneToLoad = GetSceneNameFromProgress(0);
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogError("[ModuleSceneLoader] Failed to reset progress: " + task.Exception);
                }
            });
    }

    public void CancelReset()
    {
        if (resetPopup != null)
        {
            resetPopup.SetActive(false);
            Debug.Log("[ModuleSceneLoader] Reset cancelled. Popup closed.");
        }
    }
}
