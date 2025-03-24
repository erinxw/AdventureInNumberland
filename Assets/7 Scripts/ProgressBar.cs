using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private DatabaseReference dbReference;
    public Slider progressBar;
    public GameObject progressBarHighlight;
    public int totalActivities = 5; // Total activities for full progress
    private int completedActivities;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadProgress();
    }

    public void CompleteActivity()
    {
        Debug.Log("CompleteActivity() called!");

        if (completedActivities < totalActivities)
        {
            completedActivities++;
            UpdateProgressBar();
            SaveProgressToDatabase();

            progressBarHighlight.SetActive(true);
            Invoke(nameof(HideHighlight), 1.0f);
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
        dbReference.Child("users").Child(userId).Child("progress").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    completedActivities = int.Parse(task.Result.Value.ToString());
                    UpdateProgressBar();
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

        dbReference.Child("users").Child(userId).Child("progress").SetValueAsync(completedActivities)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save progress: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Progress saved successfully! New Value: " + completedActivities);
                }
            });
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.SetValueWithoutNotify(completedActivities);
            Debug.Log("Slider updated! New Value: " + progressBar.value);
        }
        else
        {
            Debug.LogError("ProgressBar is not assigned in the Inspector!");
        }
    }

    private void HideHighlight()
    {
        progressBarHighlight.SetActive(false);
    }
}