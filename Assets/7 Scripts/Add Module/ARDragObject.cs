using UnityEngine;
using UnityEngine.InputSystem;

public class ARDragObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    public GameObject basketApple;      // Assign this in inspector: the preset apple in basket
    public LayerMask basketLayer;       // Assign in inspector
    private bool hasDropped = false;    // To prevent double dropping

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current == null || hasDropped) return;

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

            // Check for drop on basket
            Ray ray = cam.ScreenPointToRay(touch.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, basketLayer))
            {
                if (hit.collider.CompareTag("Basket"))
                {
                    DropIntoBasket();
                }
            }
        }
    }

    void DropIntoBasket()
    {
        hasDropped = true;

        // Show basket apple
        if (basketApple != null)
        {
            basketApple.SetActive(true);
        }

        // Hide dragged apple
        gameObject.SetActive(false);
    }
}
