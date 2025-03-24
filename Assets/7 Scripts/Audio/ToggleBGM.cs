using UnityEngine;
using UnityEngine.UI;

public class ToggleBGM : MonoBehaviour
{
    public Toggle musicToggle;

    private void Start()
    {
        if (musicToggle != null)
        {
            // Load saved state
            AudioManager audioManager = FindAnyObjectByType<AudioManager>();
            if (audioManager != null)
            {
                bool isMusicOn = PlayerPrefs.GetInt("BGM", 1) == 1;
                musicToggle.isOn = isMusicOn;
                audioManager.SetMusicState(isMusicOn);
            }

            // Listen for toggle changes
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        }
        else
        {
            Debug.LogError("Music Toggle is not assigned in ToggleBGM!");
        }
    }

    void ToggleMusic(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.ToggleMusic();
        }
        else
        {
            Debug.LogError("AudioManager not found!");
        }
    }
}
