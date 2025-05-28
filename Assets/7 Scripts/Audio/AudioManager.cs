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
        // Load from PlayerPrefs (optional fallback before Firebase login)
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        backgroundMusicSource.mute = !isBgmOn;
        buttonClickSource.mute = !isSfxOn;
    }

    public void ToggleBGM()
    {
        isBgmOn = !isBgmOn;
        backgroundMusicSource.mute = !isBgmOn;

        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        buttonClickSource.mute = !isSfxOn;

        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetBGMState(bool isOn)
    {
        isBgmOn = isOn;
        backgroundMusicSource.mute = !isBgmOn;

        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXState(bool isOn)
    {
        isSfxOn = isOn;
        buttonClickSource.mute = !isSfxOn;

        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayButtonClickSound()
    {
        if (isSfxOn && buttonClickSource != null)
        {
            buttonClickSource.Play();
        }
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
}
