using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;  // Assign your prefab in the inspector
    public ARRaycastManager raycastManager;  // Assign ARRaycastManager in inspector

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;  // Get first hit position

                    // Spawn object at detected plane position
                    Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
                }
            }
        }
    }
}
