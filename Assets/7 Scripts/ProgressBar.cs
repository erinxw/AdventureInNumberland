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
    public int totalActivities = 5;

    private int completedActivities;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Hide progress UI until data is loaded
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
        if (progressBarHighlight != null)
            progressBarHighlight.SetActive(false);

        LoadProgress();
    }

    public void AddCompletedActivity()
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

        dbReference.Child("users").Child(userId).Child("countProgress").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        completedActivities = Convert.ToInt32(task.Result.Value);
                    }
                    else
                    {
                        completedActivities = 0;
                    }

                    UpdateProgressBar();

                    // Now show the progress bar
                    if (progressBar != null)
                        progressBar.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("Failed to load progress from Firebase: " + task.Exception);
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

        dbReference.Child("users").Child(userId).Child("countProgress").SetValueAsync(completedActivities)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Progress saved: countProgress = {completedActivities}");
                }
                else
                {
                    Debug.LogError("Failed to save progress to Firebase: " + task.Exception);
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
