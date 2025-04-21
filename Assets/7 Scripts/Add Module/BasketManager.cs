using UnityEngine;
using System.Collections.Generic;

public class BasketManager : MonoBehaviour
{
    public List<Transform> basketSlots;
    private int currentSlot = 0;

    public bool TrySnapApple(GameObject apple)
    {
        if (currentSlot >= basketSlots.Count)
        {
            Debug.Log("No more slots available!");
            return false;
        }

        Transform slot = basketSlots[currentSlot];
        apple.transform.position = slot.position;
        apple.transform.rotation = slot.rotation;

        // Optional: Disable dragging script so it can’t be moved again
        var dragScript = apple.GetComponent<ARDragObject>();
        if (dragScript != null) dragScript.enabled = false;

        currentSlot++;
        return true;
    }
}
