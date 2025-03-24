using UnityEngine;
using System.Collections;

public class CountingTutorialDialogues : MonoBehaviour
{
    public Animator talkingAnimator; // Animator for talking animation
    public Animator tutorialAnimator; // Animator for tutorial animation
    public GameObject tutorialObject; // Object that appears in second audio
    public AudioSource[] audioSources; // Array of 3 AudioSources (from separate objects)
    public float silenceThreshold = 0.02f;
    public float checkInterval = 0.1f;

    private int currentAudioIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        tutorialObject.SetActive(false); // Hide tutorial object at start
        PlayNextDialogue();
    }

    public void PlayNextDialogue()
    {
        if (currentAudioIndex < audioSources.Length)
        {
            audioSources[currentAudioIndex].Play();
            talkingAnimator.speed = 1; // Start talking animation

            // Show tutorial animation only on second audio (index 1)
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
    }

    IEnumerator ManageAnimationPauses(AudioSource currentAudioSource)
    {
        while (currentAudioSource.isPlaying)
        {
            float[] samples = new float[256];
            currentAudioSource.GetOutputData(samples, 0);
            float volume = GetAverageVolume(samples);

            if (volume < silenceThreshold)
            {
                if (!isPaused)
                {
                    talkingAnimator.speed = 0; // Pause talking animation on silence
                    isPaused = true;
                }
            }
            else
            {
                if (isPaused)
                {
                    talkingAnimator.speed = 1; // Resume talking animation when voice resumes
                    isPaused = false;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }

        talkingAnimator.speed = 0; // Stop animation when audio ends
        currentAudioIndex++;

        if (currentAudioIndex < audioSources.Length)
        {
            PlayNextDialogue(); // Play the next audio
        }
        else
        {
            tutorialObject.SetActive(false); // Hide tutorial object after all audios finish
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
}
