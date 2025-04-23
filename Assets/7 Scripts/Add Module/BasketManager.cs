using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public Animator animator; // Drag your object with the Animator here in the Inspector
    public int totalItems; // Total number of items to collect
    private int itemsCollected = 0;

    public void ItemCollected()
    {
        itemsCollected++;
        Debug.Log("Item collected! Total: " + itemsCollected);

        if (itemsCollected >= totalItems)
        {
            Debug.Log("All items collected! Playing animation.");
            animator.SetTrigger("ShowChoices");
        }
    }
}
