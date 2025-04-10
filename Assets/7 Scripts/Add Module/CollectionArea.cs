using UnityEngine;

public class CollectionArea : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("CollectionArea script initialized.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered collection area: " + other.gameObject.name);

        if (other.CompareTag("FoodItem"))
        {
            Debug.Log("Collected: " + other.gameObject.name);
            other.gameObject.SetActive(false); // Hide the object when collected
        }
        else
        {
            Debug.Log("Object is not collectible.");
        }
    }
}
