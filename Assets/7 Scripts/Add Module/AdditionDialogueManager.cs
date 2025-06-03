using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AdditionDialogueManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    public AudioSource[] basketAudios;
    public AudioSource addFinalAudio;
    public int totalItems;
    public Animator revealAnimator;
    public string revealTriggerName = "ShowChoices";

    private int currentAudioIndex = 0;
    private bool isPaused = false;
    private float silenceThreshold = 0.02f;
    private float checkInterval = 0.1f;
    private Collider[] colliders;
    private bool isPLayingFinalAudio = false;
    private Queue<AudioSource> basketAudioQueue = new Queue<AudioSource>();

    private int itemsCollected = 0;
    private bool isBasketAudioPlaying = false;
    private bool isFinalAudioPlaying = false;
    private bool hasTriggeredFinalStep = false;

    private void Awake()
    {
        GameObject[] foodItems = GameObject.FindGameObjectsWithTag("FoodItem");
        colliders = foodItems
            .Select(obj => obj.GetComponent<Collider>())
            .Where(collider => collider != null)
            .ToArray();
    }

    void Start()
    {
        PlayNextDialogue();
    }

    void Update()
    {
        if (isPLayingFinalAudio && !addFinalAudio.isPlaying)
        {
            SetInteraction(true);
            isPLayingFinalAudio = false;
        }
    }

    public void PlayNextDialogue()
    {
        SetInteraction(false); // Disable interaction while playing audio

        if (currentAudioIndex < audioSources.Length)
        {
            audioSources[currentAudioIndex].Play();
            StartCoroutine(ManageAnimationPauses(audioSources[currentAudioIndex]));
        }
    }

    public void PlayAddFinalAudio()
    {
        if (addFinalAudio != null)
        {
            SetInteraction(false);
            addFinalAudio.Play();
            isPLayingFinalAudio = true;
            isFinalAudioPlaying = true;
            StartCoroutine(ManageAnimationPauses(addFinalAudio));
        }
    }

    IEnumerator ManageAnimationPauses(AudioSource currentAudioSource)
    {
        while (currentAudioSource.isPlaying)
        {
            float[] samples = new float[256];
            currentAudioSource.GetOutputData(samples, 0);
            float volume = GetAverageVolume(samples);

            isPaused = volume < silenceThreshold;

            yield return new WaitForSeconds(checkInterval);
        }

        currentAudioIndex++;

        if (currentAudioIndex < audioSources.Length)
        {
            PlayNextDialogue();
        }
        else
        {
            OnAllAudiosFinished();
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

    private void SetInteraction(bool enabled)
    {
        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }

    public void OnAllAudiosFinished()
    {
        if (totalItems == 0 && !hasTriggeredFinalStep)
        {
            hasTriggeredFinalStep = true;

            if (revealAnimator != null)
                revealAnimator.SetTrigger(revealTriggerName);
        }
        if (!isBasketAudioPlaying && !isFinalAudioPlaying)
        {
            SetInteraction(true);
        }
    }

    public void ItemCollected()
    {
        SetInteraction(false);

        if (itemsCollected < basketAudios.Length)
        {
            basketAudioQueue.Enqueue(basketAudios[itemsCollected]);
        }

        itemsCollected++;

        if (!isBasketAudioPlaying)
        {
            StartCoroutine(PlayBasketAudioQueue());
        }

        if (itemsCollected >= totalItems && !isFinalAudioPlaying)
        {
            StartCoroutine(PlayFinalDialogue());
        }
    }

    IEnumerator PlayBasketAudioQueue()
    {
        isBasketAudioPlaying = true;

        while (basketAudioQueue.Count > 0)
        {
            AudioSource current = basketAudioQueue.Dequeue();
            yield return StartCoroutine(PlayAudio(current));
        }

        isBasketAudioPlaying = false;

        if (!isFinalAudioPlaying)
        {
            SetInteraction(true);
        }
    }

    IEnumerator PlayFinalDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        PlayAddFinalAudio();

        yield return new WaitWhile(() => addFinalAudio.isPlaying);

        if (!hasTriggeredFinalStep && revealAnimator != null)
        {
            hasTriggeredFinalStep = true;
            revealAnimator.SetTrigger(revealTriggerName);
            Debug.Log("Reveal animation triggered.");
        }
    }

    IEnumerator PlayAudio(AudioSource audio)
    {
        if (audio != null)
        {
            audio.Play();
            yield return new WaitWhile(() => audio.isPlaying);
        }
    }
}
