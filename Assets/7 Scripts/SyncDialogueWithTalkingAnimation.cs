using UnityEngine;
using System.Collections;

public class SyncDialogueWithTalkingAnimation : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public float silenceThreshold = 0.02f; // Adjust this based on testing
    public float checkInterval = 0.1f; // How often to check audio volume

    private bool isPaused = false;

    public void PlayDialogueWithAnimation(AudioClip dialogueClip)
    {
        if (dialogueClip != null)
        {
            audioSource.clip = dialogueClip;
            audioSource.Play();
            animator.speed = 1; // Start animation

            StartCoroutine(ManageAnimationPauses());
        }
    }

    IEnumerator ManageAnimationPauses()
    {
        while (audioSource.isPlaying)
        {
            float[] samples = new float[256];
            audioSource.GetOutputData(samples, 0);
            float volume = GetAverageVolume(samples);

            if (volume < silenceThreshold)
            {
                if (!isPaused)
                {
                    animator.speed = 0; // Pause animation on silence
                    isPaused = true;
                }
            }
            else
            {
                if (isPaused)
                {
                    animator.speed = 1; // Resume animation when voice resumes
                    isPaused = false;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }

        animator.speed = 0; // Stop animation when audio ends
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
