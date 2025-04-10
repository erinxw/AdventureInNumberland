using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ARDragObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private bool isDragging = false;
    private Vector2 touchStartPos;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager is missing! Make sure AR Session Origin has an ARRaycastManager.");
        }

        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is missing! Ensure your scene has a tagged 'MainCamera'.");
        }
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.isPressed)
        {
            Vector2 touchPosition = touch.position.ReadValue();

            if (!isDragging)
            {
                HandleInput(touchPosition);
            }
            else
            {
                MoveObject(touchPosition);
            }
        }
        else
        {
            isDragging = false;
        }
    }

    void HandleInput(Vector2 screenPosition)
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform) // Check if tapped object is this object
            {
                isDragging = true;
                Debug.Log("Dragging started on: " + gameObject.name);
            }
        }
    }

    void MoveObject(Vector2 screenPosition)
    {
        if (raycastManager == null) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            transform.position = hits[0].pose.position;
            Debug.Log("Object moved to: " + transform.position);
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any AR plane.");
        }
    }
}
