using UnityEngine;

public class ModulesPageManager : MonoBehaviour
{
    void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.LoadUserAudioSettings(); // Apply Firebase audio settings here
        }
    }
}
