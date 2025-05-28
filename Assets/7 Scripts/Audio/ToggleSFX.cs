using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

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
            Debug.LogError("SFX Toggle is not assigned!");
            return;
        }

        // Load saved state from Firebase
        dbRef.Child("users").Child(userId).Child("settings").Child("sfx").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    bool isSfxOn = bool.Parse(task.Result.Value.ToString());
                    sfxToggle.isOn = isSfxOn;

                    AudioManager audioManager = FindAnyObjectByType<AudioManager>();
                    if (audioManager != null)
                        audioManager.SetSFXState(isSfxOn);
                }
                else
                {
                    sfxToggle.isOn = true; // Default ON
                }

                // Add listener only after value is set
                sfxToggle.onValueChanged.AddListener(ToggleSoundEffects);
            });
    }

    void ToggleSoundEffects(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
            audioManager.SetSFXState(isOn);

        dbRef.Child("users").Child(userId).Child("settings").Child("sfx").SetValueAsync(isOn)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted || task.IsFaulted)
                    Debug.LogError("Failed to save SFX: " + task.Exception);
            });
    }
}
