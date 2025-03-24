using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class CameraPermissionManager : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private bool isPermissionGranted = false;

    void Start()
    {
        // Request camera permission only in the introduction scene
        if (SceneManager.GetActiveScene().name == "IntroductionPageSceneAR")
        {
            RequestCameraPermission();
        }
    }

    private void RequestCameraPermission()
    {
        // Check if we already have permission
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            isPermissionGranted = true;
            InitializeCamera();
        }
        else
        {
            // Request camera permission if not granted yet
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

    void Update()
    {
        // Check permission status after the request
        if (Permission.HasUserAuthorizedPermission(Permission.Camera) && !isPermissionGranted)
        {
            isPermissionGranted = true;
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        // Initialize the WebCamTexture only after permission is granted
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();  // Start the camera feed
        Debug.Log("Camera initialized.");
    }
}