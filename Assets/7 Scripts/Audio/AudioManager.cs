using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance
    public AudioSource backgroundMusicSource; // Assign in Inspector
    public AudioSource buttonClickSource;     // Assign in Inspector

    private bool isBgmOn = true;
    private bool isSfxOn = true;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initial fallback if Firebase hasn't loaded yet
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        ApplyAudioStates();
    }

    public void LoadUserAudioSettings()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("User not logged in. Using PlayerPrefs for audio settings.");
            return;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(user.UserId)
            .Child("settings")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result != null)
                {
                    var snapshot = task.Result;

                    if (snapshot.HasChild("bgm"))
                        isBgmOn = bool.Parse(snapshot.Child("bgm").Value.ToString());
                    if (snapshot.HasChild("sfx"))
                        isSfxOn = bool.Parse(snapshot.Child("sfx").Value.ToString());

                    ApplyAudioStates();

                    // Sync with PlayerPrefs (optional)
                    PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
                    PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("Failed to load Firebase audio settings, using fallback.");
                }
            });
    }

    public void ToggleBGM()
    {
        isBgmOn = !isBgmOn;
        backgroundMusicSource.mute = !isBgmOn;
        SaveAudioSetting("bgm", isBgmOn);
    }

    public void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        buttonClickSource.mute = !isSfxOn;
        SaveAudioSetting("sfx", isSfxOn);
    }

    public void SetBGMState(bool isOn)
    {
        isBgmOn = isOn;
        backgroundMusicSource.mute = !isBgmOn;
        SaveAudioSetting("bgm", isBgmOn);
    }

    public void SetSFXState(bool isOn)
    {
        isSfxOn = isOn;
        buttonClickSource.mute = !isSfxOn;
        SaveAudioSetting("sfx", isSfxOn);
    }

    public void PlayButtonClickSound()
    {
        if (isSfxOn && buttonClickSource != null)
        {
            buttonClickSource.Play();
        }
    }

    private void SaveAudioSetting(string key, bool value)
    {
        PlayerPrefs.SetInt(key.ToUpper(), value ? 1 : 0);
        PlayerPrefs.Save();

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            FirebaseDatabase.DefaultInstance
                .GetReference("users")
                .Child(user.UserId)
                .Child("settings")
                .Child(key)
                .SetValueAsync(value);
        }
    }

    private void ApplyAudioStates()
    {
        backgroundMusicSource.mute = !isBgmOn;
        buttonClickSource.mute = !isSfxOn;
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("LunaDialogue") != null)
        {
            backgroundMusicSource.volume = 0.15f;
        }
        else
        {
            backgroundMusicSource.volume = 1.0f;
        }
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
