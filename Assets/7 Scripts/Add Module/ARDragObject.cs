using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARDragObject : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    private Vector2 touchPosition;
    private bool isDragging = false;

    void Start()
    {
        // Find the ARRaycastManager in the scene
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        touchPosition = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            // Check if the touch hits this object
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                }
            }
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            MoveObject();
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            isDragging = false;
        }
    }

    void MoveObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            transform.position = hits[0].pose.position;
        }
    }
}
