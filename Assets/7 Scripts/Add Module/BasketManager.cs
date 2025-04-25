using UnityEngine;
using System.Collections;

public class BasketManager : MonoBehaviour
{
    public Animator animator;
    public int totalItems;
    private int itemsCollected = 0;

    public AudioSource[] audioSources; // Set up in Inspector: "One", "Two", etc.
    private int currentAudioIndex = 0;
    private bool isPlayingAudio = false;

    void Start()
    {
        animator.ResetTrigger("ShowChoices");
        animator.Play("Idle");
    }

    public void ItemCollected()
    {
        itemsCollected++;
        Debug.Log("Item collected! Total: " + itemsCollected);

        if (currentAudioIndex < audioSources.Length)
        {
            StartCoroutine(PlayNextAudio());
        }

        // Animation will be triggered from the coroutine when audio finishes
    }

    IEnumerator PlayNextAudio()
    {
        isPlayingAudio = true;

        audioSources[currentAudioIndex].Play();
        yield return new WaitForSeconds(audioSources[currentAudioIndex].clip.length);
        currentAudioIndex++;

        isPlayingAudio = false;

        if (itemsCollected >= totalItems && currentAudioIndex >= audioSources.Length)
        {
            Debug.Log("All items collected & audio done! Playing animation.");
            animator.SetTrigger("ShowChoices");
        }
    }
}
