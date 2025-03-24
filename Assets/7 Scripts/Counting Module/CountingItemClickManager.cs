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

    private int currentAudioIndex = 0;
    private HashSet<GameObject> clickedItems = new HashSet<GameObject>();
    private GameObject[] allFoodItems;
    private bool allItemsClicked = false;
    private bool isPlayingAudio = false;
    private bool completed = false; // Prevent multiple triggers

    void Start()
    {
        allFoodItems = GameObject.FindGameObjectsWithTag("FoodItem");
        Debug.Log("Found " + allFoodItems.Length + " food items in the scene.");
    }

    void Update()
    {
        if (completed) return;

        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            Debug.Log("[Update] Touch detected at position: " + touchPosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("[Update] Raycast hit: " + hit.collider.name);

                if (hit.collider.CompareTag("FoodItem"))
                {
                    if (!clickedItems.Contains(hit.collider.gameObject))
                    {
                        Debug.Log("[Update] Marking item: " + hit.collider.name);
                        MarkItem(hit.collider.gameObject);

                        if (!isPlayingAudio)
                        {
                            Debug.Log("[Update] Starting PlayNextAudio coroutine");
                            StartCoroutine(PlayNextAudio());
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[Update] Item already clicked: " + hit.collider.name);
                    }
                }
            }
            else
            {
                Debug.Log("[Update] No valid object hit.");
            }
        }
    }

    void MarkItem(GameObject item)
    {
        Debug.Log("[MarkItem] Processing item: " + item.name);

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
        Debug.Log("[MarkItem] Total clicked items: " + clickedItems.Count + "/" + allFoodItems.Length);

        if (clickedItems.Count == allFoodItems.Length && !completed)
        {
            completed = true;
            allItemsClicked = true;
            Debug.Log("[MarkItem] All items clicked! Calling CompleteActivity().");

            if (progressBar != null)
            {
                progressBar.CompleteActivity();
            }
            else
            {
                Debug.LogError("[MarkItem] ProgressBar is NULL! Did you assign it in the Inspector?");
            }
        }
    }


    IEnumerator PlayNextAudio()
    {
        isPlayingAudio = true;

        while (currentAudioIndex < audioSources.Length)
        {
            audioSources[currentAudioIndex].Play();
            yield return new WaitForSeconds(audioSources[currentAudioIndex].clip.length);
            currentAudioIndex++;
        }

        isPlayingAudio = false;

        // Load next scene only after all items are clicked and audio finishes
        if (allItemsClicked)
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
