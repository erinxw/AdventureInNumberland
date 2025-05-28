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

        // Load saved state from Firebase
        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    bool isBgmOn = bool.Parse(task.Result.Value.ToString());
                    bgmToggle.isOn = isBgmOn;

                    AudioManager audioManager = FindAnyObjectByType<AudioManager>();
                    if (audioManager != null)
                        audioManager.SetBGMState(isBgmOn);
                }
                else
                {
                    bgmToggle.isOn = true; // Default ON
                }

                // Add listener only after value is set
                bgmToggle.onValueChanged.AddListener(ToggleBgMusic);
            });
    }

    void ToggleBgMusic(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
            audioManager.SetBGMState(isOn);

        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").SetValueAsync(isOn)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted || task.IsFaulted)
                    Debug.LogError("Failed to save BGM: " + task.Exception);
            });
    }
}
