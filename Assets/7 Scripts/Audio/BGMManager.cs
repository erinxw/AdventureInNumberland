using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance; // Singleton instance
    public AudioSource backgroundMusic; // Reference to the background music AudioSource

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Makes this GameObject persistent across scenes

            // Start playing the background music
            if (backgroundMusic != null && !backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }
}
