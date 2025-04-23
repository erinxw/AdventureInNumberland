using UnityEngine;
using UnityEngine.InputSystem;

public class ARDragObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(touch.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                offset = transform.position - hit.point;
                isDragging = true;
            }
        }
        else if (touch.press.isPressed && isDragging)
        {
            Ray ray = cam.ScreenPointToRay(touch.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = hit.point + offset;
            }
        }
        else if (touch.press.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
        }
    }
}
