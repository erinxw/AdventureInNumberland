using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    public static AudioInitializer instance;

    public AudioSource backgroundMusicSource;
    public AudioSource buttonClickSource;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
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

    public void PlayButtonClick()
    {
        if (buttonClickSource != null)
        {
            buttonClickSource.Play();
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
