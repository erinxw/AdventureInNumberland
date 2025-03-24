using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class DeleteProfileManager : MonoBehaviour
{
    public Button deleteButton; // Reference to the Delete button

    private FirebaseAuth auth;
    private DatabaseReference reference;

    private void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Firebase initialized successfully.");

        // Add listener to the Delete button only after Firebase initialization
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(DeleteUserProfile);
        }
        else
        {
            Debug.LogError("Delete button is not assigned!");
        }
    }

    // Delete the user's profile when the Delete button is clicked
    public void DeleteUserProfile()
    {
        if (auth == null)
        {
            Debug.LogError("FirebaseAuth is not initialized.");
            return;
        }

        if (auth.CurrentUser == null)
        {
            Debug.LogError("No user is currently logged in.");
            return;
        }

        string userId = auth.CurrentUser.UserId; // Get the current user's unique ID
        Debug.Log($"Attempting to delete profile for user {userId}");

        // Delete user data from Realtime Database
        DeleteUserDataFromDatabase(userId);
    }

    // Delete user data from Firebase Realtime Database
    private void DeleteUserDataFromDatabase(string userId)
    {
        if (reference == null)
        {
            Debug.LogError("DatabaseReference is not initialized.");
            return;
        }

        reference.Child("users").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error deleting user data from Realtime Database: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("User data deletion in Realtime Database was canceled.");
            }
            else
            {
                Debug.Log("User data deleted successfully from Realtime Database.");

                // After deleting user data, delete user authentication
                DeleteUserAuthentication();
            }
        });
    }

    // Optionally delete the user's authentication details (completely removing their profile)
    private void DeleteUserAuthentication()
    {
        auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error deleting user authentication details: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("User authentication deletion was canceled.");
            }
            else
            {
                Debug.Log("User authentication details deleted successfully.");

                // Load the landing page after successful deletion
                Invoke("LoadLandingPage", 2f); // Delay the scene load by 2 seconds
            }
        });
    }

    // Load the landing page scene
    private void LoadLandingPage()
    {
        SceneManager.LoadScene("LandingPageScene");
    }
}
