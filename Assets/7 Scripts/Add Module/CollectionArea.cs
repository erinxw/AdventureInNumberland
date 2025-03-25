using UnityEngine;

public class CollectionArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Draggable"))
        {
            Debug.Log("Object Collected!");
            other.gameObject.SetActive(false); // Hide object upon collection
        }
    }
}
