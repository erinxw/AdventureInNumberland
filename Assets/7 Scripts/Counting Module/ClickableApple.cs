using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClickableApple : MonoBehaviour
{
    private AudioSource audioSource;   // AudioSource to play sound
    public AudioClip clickSound;       // Audio clip for feedback
    public string nextSceneName;       // Name of the next scene to load

    private bool isAudioPlaying = false; // Prevent overlapping sounds

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Missing AudioSource on " + gameObject.name);
        }
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleInput(touchPosition);
        }
    }

    void HandleInput(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            OnAppleClicked();
        }
    }

    void OnAppleClicked()
    {
        if (isAudioPlaying || clickSound == null || audioSource == null) return;

        isAudioPlaying = true;
        audioSource.PlayOneShot(clickSound);
        Debug.Log("Apple clicked!");

        // Reset audio flag after the sound finishes
        Invoke(nameof(ResetAudioFlag), clickSound.length);

        // Load the next scene after the clip finishes playing
        Invoke(nameof(LoadNextScene), clickSound.length);
    }

    void ResetAudioFlag() => isAudioPlaying = false;

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name not set in " + gameObject.name);
        }
    }
}
