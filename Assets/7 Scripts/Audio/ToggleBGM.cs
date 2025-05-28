using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

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
            Debug.LogError("BGM Toggle is not assigned in ToggleBGM!");
            return;
        }

        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                bool isBgmOn = bool.Parse(task.Result.Value.ToString());

                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    bgmToggle.isOn = isBgmOn;

                    AudioManager audioManager = FindAnyObjectByType<AudioManager>();
                    if (audioManager != null)
                        audioManager.SetBGMState(isBgmOn);

                    bgmToggle.onValueChanged.AddListener(ToggleBgMusic);
                });
            }
            else
            {
                Debug.Log("No saved BGM setting found, defaulting to ON.");
                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    bgmToggle.isOn = true;
                    bgmToggle.onValueChanged.AddListener(ToggleBgMusic);
                });
            }
        });
    }

    void ToggleBgMusic(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetBGMState(isOn);
        }

        dbRef.Child("users").Child(userId).Child("settings").Child("bgm").SetValueAsync(isOn)
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("BGM saved: " + isOn);
                else
                    Debug.LogError("Failed to save BGM: " + task.Exception);
            });
    }
}
