using UnityEngine;
using System.Collections;

public class SyncAnimWDialogue : MonoBehaviour
{
    public Animator talkingAnimator; // Animator for talking animation
    public AudioSource[] audioSources; // Array of AudioSources for dialogue
    public AudioSource subFinalAudio;

    private int currentAudioIndex = 0;
    private bool isPaused = false;
    private float silenceThreshold = 0.02f;
    private float checkInterval = 0.1f;

    void Start()
    {
        PlayNextDialogue();
    }

    public void PlayNextDialogue()
    {
        if (currentAudioIndex < audioSources.Length)
        {
            audioSources[currentAudioIndex].Play();
            talkingAnimator.speed = 1; // Start talking animation
            StartCoroutine(ManageAnimationPauses(audioSources[currentAudioIndex]));
        }
    }

    public void PlaySubFinalAudio()
    {
        if (subFinalAudio != null)
        {
            subFinalAudio.Play();
            talkingAnimator.speed = 1; // Start talking animation
            StartCoroutine(ManageAnimationPauses(subFinalAudio));
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
