using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class LogoutManager : MonoBehaviour
{
    public Button logoutButton;  // Reference to the logout button

    private FirebaseAuth auth;

    private void Start()
    {
        // Initialize Firebase Authentication
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase initialized successfully.");

                // Add listener to the logout button
                logoutButton.onClick.AddListener(LogoutUser);
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    public void LogoutUser()
    {
        if (auth != null)
        {
            auth.SignOut();  // Sign out the user
            Debug.Log("User signed out successfully.");
        }
        else
        {
            Debug.LogError("FirebaseAuth instance is null.");
        }
    }
}
