using UnityEngine;
using UnityEngine.InputSystem;

public class ARDragObject : MonoBehaviour
{
    private bool isDragging = false;  // To check if the object is being dragged
    private Vector2 touchStartPosition;  // Store touch start position
    private Vector3 objectStartPosition;  // Store object's initial position
    private Vector3 offset;  // The difference between the object position and touch position

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleInput(touchPosition);
        }
    }

    void HandleInput(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                DragApple(screenPosition, hit);
            }
        }
    }

    void DragApple(Vector2 touchPosition, RaycastHit hit)
    {
        if (!isDragging)
        {
            isDragging = true;
            touchStartPosition = touchPosition;
            objectStartPosition = transform.position;
            offset = objectStartPosition - hit.point;
        }

        // Calculate the new position based on the offset and touch movement
        Vector3 newWorldPosition = hit.point + offset;
        transform.position = newWorldPosition;

        Debug.Log("Dragging to: " + newWorldPosition);
    }
}
