using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARObjectPlacement : MonoBehaviour
{
    public GameObject objectGroupPrefab;  // Prefab containing the parent GameObject with all child objects
    private ARRaycastManager raycastManager;
    private bool isObjectPlaced = false;  // Flag to prevent placing more than one group

    void Start()
    {
        // Get the ARRaycastManager component attached to this GameObject
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // If the object group has been placed already, return early
        if (isObjectPlaced) return;

        // Perform a raycast from the center of the screen to detect planes
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            // If a plane is detected, get the position and rotation for the hit
            Pose hitPose = hits[0].pose;

            // Instantiate the entire object group at the detected position
            Instantiate(objectGroupPrefab, hitPose.position, hitPose.rotation);

            // Set the flag to avoid placing more objects
            isObjectPlaced = true;
        }
    }
}
