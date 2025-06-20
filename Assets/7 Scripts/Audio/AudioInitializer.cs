using UnityEngine;
using UnityEngine.SceneManagement;

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

            // Subscribe to scene load event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Auto-destroy when reaching Modules Page
        if (scene.name == "ModulesPageScene")
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
