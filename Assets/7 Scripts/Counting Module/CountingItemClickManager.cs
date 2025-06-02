using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CountingItemClickManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    public string nextSceneName;
    public ProgressBar progressBar;
    public AudioSource instrAudio;

    private int currentAudioIndex = 0;
    private HashSet<GameObject> clickedItems = new HashSet<GameObject>();
    private GameObject[] allFoodItems;
    private bool allItemsClicked = false;
    private bool isPlayingAudio = false;
    private bool completed = false;

    void Start()
    {
        allFoodItems = GameObject.FindGameObjectsWithTag("FoodItem");
        Debug.Log("Found " + allFoodItems.Length + " food items in the scene.");
    }

    void Update()
    {
        if (completed || isPlayingAudio || IsInstrAudioPlaying()) return;

        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("FoodItem"))
                {
                    if (!clickedItems.Contains(hit.collider.gameObject))
                    {
                        MarkItem(hit.collider.gameObject);
                        StartCoroutine(PlayNextAudio());
                    }
                }
            }
        }
    }

    bool IsInstrAudioPlaying()
    {
        return instrAudio != null && instrAudio.isPlaying;
    }

    void MarkItem(GameObject item)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color newColor = renderer.material.color;
            newColor.a = 0.5f;
            renderer.material.color = newColor;
        }

        Transform tickIcon = item.transform.Find("TickIcon");
        if (tickIcon != null)
        {
            tickIcon.gameObject.SetActive(true);
        }

        clickedItems.Add(item);

        if (clickedItems.Count == allFoodItems.Length && !completed)
        {
            completed = true;
            allItemsClicked = true;

            if (progressBar != null)
            {
                progressBar.AddCompletedActivity();
            }
            else
            {
                Debug.LogError("ProgressBar is NULL! Did you assign it in the Inspector?");
            }
        }
    }

    IEnumerator PlayNextAudio()
    {
        if (currentAudioIndex >= audioSources.Length)
        {
            if (allItemsClicked) LoadNextScene();
            yield break;
        }

        isPlayingAudio = true;

        audioSources[currentAudioIndex].Play();
        yield return new WaitForSeconds(audioSources[currentAudioIndex].clip.length);
        currentAudioIndex++;

        isPlayingAudio = false;

        if (allItemsClicked && currentAudioIndex >= audioSources.Length)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        Debug.Log("Loading next scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
