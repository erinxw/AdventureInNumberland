using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;

public class ToggleSFX : MonoBehaviour
{
    public Toggle sfxToggle;
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

        if (sfxToggle == null)
        {
            Debug.LogError("SFX Toggle is not assigned in ToggleSFX!");
            return;
        }

        // Load saved state from Firebase
        dbRef.Child("users").Child(userId).Child("settings").Child("sfx").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                bool isSfxOn = bool.Parse(task.Result.Value.ToString());

                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    sfxToggle.isOn = isSfxOn;

                    AudioManager audioManager = FindAnyObjectByType<AudioManager>();
                    if (audioManager != null)
                        audioManager.SetSFXState(isSfxOn);

                    // Add listener AFTER setting toggle
                    sfxToggle.onValueChanged.AddListener(ToggleSoundEffects);
                });
            }
            else
            {
                Debug.Log("No saved SFX setting found, defaulting to ON.");
                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    sfxToggle.isOn = true;
                    sfxToggle.onValueChanged.AddListener(ToggleSoundEffects);
                });
            }
        });
    }

    void ToggleSoundEffects(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetSFXState(isOn);
        }

        dbRef.Child("users").Child(userId).Child("settings").Child("sfx").SetValueAsync(isOn)
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("SFX saved: " + isOn);
                else
                    Debug.LogError("Failed to save SFX: " + task.Exception);
            });
    }
}
