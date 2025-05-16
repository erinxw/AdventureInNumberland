using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasketManager : MonoBehaviour
{
    public Animator animator;
    public int totalItems;
    public AudioSource[] audioSources;

    private int itemsCollected = 0;
    private int currentAudioIndex = 0;
    private bool isPlayingAudio = false;
    private Queue<int> audioQueue = new Queue<int>();

    void Start()
    {
        animator.ResetTrigger("ShowChoices");
        animator.Play("Idle");
    }

    public void ItemCollected()
    {
        itemsCollected++;
        Debug.Log("Item collected! Total: " + itemsCollected);

        // Queue audio playback
        if (currentAudioIndex < audioSources.Length)
        {
            audioQueue.Enqueue(currentAudioIndex);
            currentAudioIndex++;

            if (!isPlayingAudio)
            {
                StartCoroutine(PlayQueuedAudio());
            }
        }
    }

    IEnumerator PlayQueuedAudio()
    {
        isPlayingAudio = true;

        while (audioQueue.Count > 0)
        {
            int index = audioQueue.Dequeue();
            AudioSource currentAudio = audioSources[index];

            currentAudio.Play();
            yield return new WaitForSeconds(currentAudio.clip.length);
        }

        isPlayingAudio = false;

        if (itemsCollected >= totalItems && currentAudioIndex >= audioSources.Length)
        {
            Debug.Log("All items collected & audio done! Playing animation.");
            animator.SetTrigger("ShowChoices");
        }
    }
}
