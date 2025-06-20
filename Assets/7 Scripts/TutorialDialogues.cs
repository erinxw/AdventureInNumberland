using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountingTutorialDialogues : MonoBehaviour
{
    public Animator talkingAnimator;         // Luna's Animator
    public Animator tutorialAnimator;        // Animator for tutorial animation
    public GameObject tutorialObject;        // Object that appears in second audio
    public AudioSource[] audioSources;       // Array of 3 AudioSources
    public Button nextButton;                // Button to AR activity
    public float silenceThreshold = 0.02f;   // Volume threshold to detect silence
    public float checkInterval = 0.1f;       // How often to check for silence
    public bool isDemoMode = false;          // Enable skip for demo purposes

    private int currentAudioIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        tutorialObject.SetActive(false);             // Hide tutorial at start
        talkingAnimator.gameObject.SetActive(false); // Hide Luna at start

        if (isDemoMode)
        {
            nextButton.interactable = true; // Allow skipping
            nextButton.onClick.AddListener(SkipTutorial);
        }
        else
        {
            nextButton.interactable = false; // Wait for dialogue to finish
        }

        PlayNextDialogue();
    }

    public void PlayNextDialogue()
    {
        if (currentAudioIndex < audioSources.Length)
        {
            audioSources[currentAudioIndex].Play();

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentAudioIndex == 0 || currentAudioIndex == 2 ||
                (currentAudioIndex == 1 && currentScene == "SubTutorial"))
            {
                talkingAnimator.gameObject.SetActive(true);
                talkingAnimator.speed = 1;
            }
            else
            {
                talkingAnimator.gameObject.SetActive(false);
            }

            if (currentAudioIndex == 1)
            {
                tutorialObject.SetActive(true);
                tutorialAnimator.Play("YourTutorialAnimation");
            }
            else
            {
                tutorialObject.SetActive(false);
            }

            StartCoroutine(ManageAnimationPauses(audioSources[currentAudioIndex]));
        }
        else
        {
            nextButton.interactable = true;
        }
    }

    IEnumerator ManageAnimationPauses(AudioSource currentAudioSource)
    {
        while (currentAudioSource.isPlaying)
        {
            float[] samples = new float[256];
            currentAudioSource.GetOutputData(samples, 0);
            float volume = GetAverageVolume(samples);

            if (talkingAnimator.gameObject.activeInHierarchy)
            {
                if (volume < silenceThreshold && !isPaused)
                {
                    talkingAnimator.speed = 0;
                    isPaused = true;
                }
                else if (volume >= silenceThreshold && isPaused)
                {
                    talkingAnimator.speed = 1;
                    isPaused = false;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }

        if (talkingAnimator.gameObject.activeInHierarchy)
            talkingAnimator.speed = 0;

        currentAudioIndex++;

        if (currentAudioIndex < audioSources.Length)
        {
            PlayNextDialogue();
        }
        else
        {
            tutorialObject.SetActive(false);
            talkingAnimator.gameObject.SetActive(false);
            nextButton.interactable = true;
        }
    }

    private float GetAverageVolume(float[] samples)
    {
        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += Mathf.Abs(sample);
        }
        return sum / samples.Length;
    }

    // Skip tutorial immediately (for demo)
    public void SkipTutorial()
    {
        StopAllCoroutines();

        foreach (var audio in audioSources)
        {
            if (audio.isPlaying)
                audio.Stop();
        }

        tutorialObject.SetActive(false);
        talkingAnimator.gameObject.SetActive(false);
        nextButton.interactable = true;

        Debug.Log("Tutorial skipped (demo mode).");
    }
}
