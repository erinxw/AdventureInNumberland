using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

public class UpdateProfileManager : MonoBehaviour
{
    public InputField emailInputField; // Reference to the email input field
    public InputField passwordInputField; // Reference to the password input field
    public Image emailInputImage; // Reference to the image associated with the email input field
    public Image passwordInputImage; // Reference to the image associated with the password input field
    public Button editButton; // Reference to the Edit button
    public Button saveButton; // Reference to the Save button

    private FirebaseAuth auth;
    private DatabaseReference reference;

    private void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully.");

                // Set initial view to non-editable (saved view)
                SetEditViewActive(false);

                // Add listeners to buttons
                editButton.onClick.AddListener(EnterEditMode);
                saveButton.onClick.AddListener(SaveData);
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    // Switch to edit mode when the Edit button is clicked
    public void EnterEditMode()
    {
        SetEditViewActive(true);
    }

    // Save data when the Save button is clicked
    public void SaveData()
    {
        // Ensure email and password are not empty before attempting update
        if (string.IsNullOrEmpty(emailInputField.text) || string.IsNullOrEmpty(passwordInputField.text))
        {
            Debug.LogError("Email or password is empty.");
            return;
        }

        // Update data in Firebase Realtime Database
        UpdateDatabase(emailInputField.text, passwordInputField.text);

        // Switch to saved view after saving
        SetEditViewActive(false);
    }

    // Toggle the UI components based on whether in edit or saved mode
    private void SetEditViewActive(bool isEditMode)
    {
        // Enable or disable input fields based on the mode
        emailInputField.interactable = isEditMode;
        passwordInputField.interactable = isEditMode;

        // Show or hide the images based on the mode
        emailInputImage.gameObject.SetActive(isEditMode); // Make image visible in edit mode
        passwordInputImage.gameObject.SetActive(isEditMode); // Make image visible in edit mode

        // Toggle buttons visibility based on the mode
        editButton.gameObject.SetActive(!isEditMode); // Hide Edit button when in edit mode
        saveButton.gameObject.SetActive(isEditMode); // Show Save button when in edit mode
    }

    // Update user data in Firebase Realtime Database
    private void UpdateDatabase(string newEmail, string password)
    {
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId; // Get the current user's unique ID
            Debug.Log($"Attempting to update user data for {userId}");

            // Create a dictionary to hold the updated user data
            Dictionary<string, object> userUpdates = new Dictionary<string, object>
            {
                { "email", newEmail },  // Update email field
                { "password", password }  // Update password field
            };

            // Update the specific fields under the user's node in Realtime Database
            reference.Child("users").Child(userId).UpdateChildrenAsync(userUpdates).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error updating data in Realtime Database: " + task.Exception);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogWarning("Data update in Realtime Database was canceled.");
                }
                else
                {
                    Debug.Log("Data updated successfully in Realtime Database.");
                }
            });
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }
    }
}