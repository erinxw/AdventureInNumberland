using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class ToggleBGM : MonoBehaviour
{
    public Toggle bgmToggle;
    private DatabaseReference dbRef;
    private string userId;

    private void Start()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogError("User not logged in.");
            return;
        }

        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        if (bgmToggle == null)
        {
            Debug.LogError("BGM Toggle is not assigned!");
            return;
        }

        // Step 1: Remove any previous listeners to avoid duplicate triggers
        bgmToggle.onValueChanged.RemoveAllListeners();

        // Step 2: Load saved state from Firebase
        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    bool isBgmOn = bool.Parse(task.Result.Value.ToString());
                    bgmToggle.isOn = isBgmOn;

                    // Set BGM state in AudioManager
                    AudioManager audioManager = FindAnyObjectByType<AudioManager>();
                    if (audioManager != null)
                        audioManager.SetBGMState(isBgmOn);
                }
                else
                {
                    // Default ON if nothing saved yet
                    bgmToggle.isOn = true;
                }

                // Step 3: Add listener only after value is applied
                bgmToggle.onValueChanged.AddListener(ToggleBgMusic);
            });
    }

    void ToggleBgMusic(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
            audioManager.SetBGMState(isOn);

        // Save to Firebase
        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").SetValueAsync(isOn)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("BGM setting saved: " + isOn);
                }
                else
                {
                    Debug.LogError("Failed to save BGM: " + task.Exception);
                }
            });
    }
}
