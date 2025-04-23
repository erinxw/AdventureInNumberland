using UnityEngine;

public class ItemSnapToBasket : MonoBehaviour
{
    public GameObject snapPointObject; // Drag it in the Inspector
    public BasketManager basketManager;
    private static int totalSnapped = 0;
    private static int totalItems;

    void Start()
    {
        if (totalItems == 0)
        {
            totalItems = GameObject.FindGameObjectsWithTag("FoodItem").Length;
            Debug.Log("Total food items: " + totalItems);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basket"))
        {
            basketManager.ItemCollected();
            Debug.Log("Item collected: " + gameObject.name);
            gameObject.SetActive(false); // Hide the item

            if (snapPointObject != null)
            {
                Debug.Log("SnapPoint reference exists: " + snapPointObject.name);
                snapPointObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("SnapPoint not assigned in Inspector!");
            }

            totalSnapped++;

            if (totalSnapped == totalItems)
            {
                Debug.Log("All items have been placed into the basket!");
            }
        }
    }
}
