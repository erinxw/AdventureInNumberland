using UnityEngine;

public class ItemSnapToBasket : MonoBehaviour
{
    public GameObject snapPointObject; // Drag it in the Inspector
    public AdditionDialogueManager additionDialogueManager;
    private static int totalSnapped = 0;
    private static int totalItems;
    private static bool totalInitialized = false;

    void Start()
    {
        if (!totalInitialized) // Only calculate total items once
        {
            totalItems = GameObject.FindGameObjectsWithTag("FoodItem").Length;
            totalInitialized = true;
            Debug.Log("Total food items initialized: " + totalItems);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basket"))
        {
            Debug.Log("Item collided with basket");

            if (additionDialogueManager != null)
            {
                additionDialogueManager.ItemCollected();
            }
            else
            {
                Debug.LogWarning("additionDialogueManager is NOT assigned on " + gameObject.name);
            }

            if (snapPointObject != null)
            {
                Debug.Log("SnapPoint reference exists: " + snapPointObject.name);
                snapPointObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("SnapPoint not assigned in Inspector for: " + gameObject.name);
            }

            totalSnapped++;
            Debug.Log($"Item snapped. Total snapped: {totalSnapped} / {totalItems}");

            gameObject.SetActive(false); // Hide the item

            if (totalSnapped == totalItems)
            {
                Debug.Log("All items have been placed into the basket!");
            }
        }
    }
}
