using UnityEngine;
using UnityEngine.UI;

public class ToggleSFX : MonoBehaviour
{
    public Toggle sfxToggle;

    private void Start()
    {
        if (sfxToggle != null)
        {
            // Load saved state
            AudioManager audioManager = FindAnyObjectByType<AudioManager>();
            if (audioManager != null)
            {
                bool isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
                sfxToggle.isOn = isSfxOn;
                audioManager.SetSFXState(isSfxOn);
            }

            // Listen for toggle changes
            sfxToggle.onValueChanged.AddListener(ToggleSoundEffects);
        }
        else
        {
            Debug.LogError("SFX Toggle is not assigned in ToggleSFX!");
        }
    }

    void ToggleSoundEffects(bool isOn)
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.ToggleSFX();
        }
        else
        {
            Debug.LogError("AudioManager not found!");
        }
    }
}
