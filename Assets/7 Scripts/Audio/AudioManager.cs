using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance
    public AudioSource backgroundMusicSource; // Assign in Inspector
    public AudioSource buttonClickSource; // Assign in Inspector

    private bool isBgmOn = true;
    private bool isSfxOn = true;

    private void Awake()
    {
        // Singleton pattern to ensure only one AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep music playing across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load saved BGM & SFX state
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        backgroundMusicSource.mute = !isBgmOn;
        buttonClickSource.mute = !isSfxOn;
    }

    public void ToggleMusic()
    {
        isBgmOn = !isBgmOn;
        backgroundMusicSource.mute = !isBgmOn;

        // Save state
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        buttonClickSource.mute = !isSfxOn;

        // Save state
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayButtonClickSound()
    {
        if (isSfxOn)
        {
            buttonClickSource.Play();
        }
    }
    public void SetMusicState(bool isOn)
    {
        backgroundMusicSource.mute = !isOn;
    }

    public void SetSFXState(bool isOn)
    {
        buttonClickSource.mute = !isOn;
    }

    void Update()
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

}
