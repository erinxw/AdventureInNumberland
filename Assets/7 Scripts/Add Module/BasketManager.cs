using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BasketManager : MonoBehaviour
{
    public Animator talkingAnimator;
    public AudioSource[] audioSources; // [0]=Intro, [1]=Instruction, [2]=Final
    public AudioSource[] basketAudios; // Feedback per collected item
    public int totalItems;

    private int itemsCollected = 0;
    private Collider[] colliders;
    private bool isPaused = false;
    private float silenceThreshold = 0.02f;
    private float checkInterval = 0.1f;

    private Queue<AudioSource> basketAudioQueue = new Queue<AudioSource>();
    private bool isBasketAudioPlaying = false;
    private bool isFinalAudioPlaying = false;

    void Awake()
    {
        GameObject[] foodItems = GameObject.FindGameObjectsWithTag("FoodItem");
        colliders = foodItems
            .Select(obj => obj.GetComponent<Collider>())
            .Where(c => c != null)
            .ToArray();
    }

    void Start()
    {
        SetInteraction(false); // Disable interaction at start
        StartCoroutine(PlayInitialDialogueSequence());
    }

    void Update()
    {
        if (isFinalAudioPlaying && !audioSources[2].isPlaying)
        {
            SetInteraction(true);
            isFinalAudioPlaying = false;
        }
    }

    IEnumerator PlayInitialDialogueSequence()
    {
        yield return StartCoroutine(PlayAudioWithLipSync(audioSources[0])); // "Welcome..."
        yield return StartCoroutine(PlayAudioWithLipSync(audioSources[1])); // "Please collect..."
        SetInteraction(true); // Enable interaction after intro
    }

    public void ItemCollected()
    {
        SetInteraction(false); // Disable interaction while feedback plays

        if (itemsCollected < basketAudios.Length)
        {
            basketAudioQueue.Enqueue(basketAudios[itemsCollected]);
        }

        itemsCollected++;

        if (!isBasketAudioPlaying)
        {
            StartCoroutine(PlayBasketAudioQueue());
        }

        // All items collected
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
            yield return StartCoroutine(PlayAudioWithLipSync(current));
        }

        isBasketAudioPlaying = false;
        if (!isFinalAudioPlaying) SetInteraction(true);
    }

    IEnumerator PlayFinalDialogue()
    {
        // Wait for any ongoing basket audio
        while (isBasketAudioPlaying)
            yield return null;

        SetInteraction(false);
        isFinalAudioPlaying = true;

        yield return StartCoroutine(PlayAudioWithLipSync(audioSources[2])); // "Great job!"
        talkingAnimator.SetTrigger("ShowChoices"); // E.g., show buttons/choices
    }

    IEnumerator PlayAudioWithLipSync(AudioSource audio)
    {
        audio.Play();
        talkingAnimator.SetTrigger("IsTalking");

        while (audio.isPlaying)
        {
            float[] samples = new float[256];
            audio.GetOutputData(samples, 0);
            float volume = GetAverageVolume(samples);

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

            yield return new WaitForSeconds(checkInterval);
        }

        talkingAnimator.speed = 0;
    }

    float GetAverageVolume(float[] samples)
    {
        float sum = 0f;
        foreach (float s in samples)
        {
            sum += Mathf.Abs(s);
        }
        return sum / samples.Length;
    }

    private void SetInteraction(bool enabled)
    {
        foreach (var collider in colliders)
        {
            if (collider != null)
                collider.enabled = enabled;
        }
    }
}
