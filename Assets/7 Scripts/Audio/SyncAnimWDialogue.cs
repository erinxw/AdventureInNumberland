using UnityEngine;
using System.Collections;
using System.Linq;

public class SyncAnimWDialogue : MonoBehaviour
{
    public Animator talkingAnimator; // Animator for mouth animation
    public AudioSource[] audioSources; // Dialogue clips
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
            TriggerTalkingAnimation();
            StartCoroutine(ManageAnimationPauses(audioSources[currentAudioIndex]));
        }
    }

    public void PlaySubFinalAudio()
    {
        if (subFinalAudio != null)
        {
            subFinalAudio.Play();
            TriggerTalkingAnimation();
            StartCoroutine(ManageAnimationPauses(subFinalAudio));
        }
    }

    public void PlayEatMouthAnimation()
    {
        Debug.Log("Triggered Eat Animation");
        if (talkingAnimator != null)
        {
            talkingAnimator.speed = 1f;
            talkingAnimator.SetTrigger("IsEating");
            StartCoroutine(ResetMouthAfterDelay(1f));
        }
    }

    private void TriggerTalkingAnimation()
    {
        if (talkingAnimator != null)
        {
            talkingAnimator.SetTrigger("IsTalking");
        }
    }

    IEnumerator ResetMouthAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        talkingAnimator.speed = 0f;
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
                    talkingAnimator.speed = 0; // Temporarily pause
                    isPaused = true;
                }
            }
            else
            {
                if (isPaused)
                {
                    talkingAnimator.speed = 1; // Resume
                    isPaused = false;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }

        talkingAnimator.speed = 0;
        currentAudioIndex++;

        if (currentAudioIndex < audioSources.Length)
        {
            PlayNextDialogue();
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
