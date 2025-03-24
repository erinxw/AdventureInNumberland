using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;

public class EditProfileManager : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField guardianPasscodeInputField;

    public Button editButton;
    public Button saveButton;

    public Image emailInputFieldImage;
    public Image passwordInputFieldImage;
    public Image guardianPasscodeInputFieldImage;

    private FirebaseAuth auth;
    private DatabaseReference reference;

    private void Start()
    {
        // Check if UI elements are assigned
        CheckUIElementAssignment();

        // Initialize Firebase and set up the database reference
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                // Set Save button to be initially disabled
                saveButton.interactable = false;
                SetInputFieldsInteractable(false);

                // Add listeners to buttons
                editButton.onClick.AddListener(EnterEditMode);
                saveButton.onClick.AddListener(SaveData);

                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    private void CheckUIElementAssignment()
    {
        if (editButton == null) Debug.LogError("Edit Button is not assigned!");
        if (saveButton == null) Debug.LogError("Save Button is not assigned!");
        if (emailInputField == null) Debug.LogError("Email InputField is not assigned!");
        if (passwordInputField == null) Debug.LogError("Password InputField is not assigned!");
        if (guardianPasscodeInputField == null) Debug.LogError("Guardian Passcode InputField is not assigned!");
        if (emailInputFieldImage == null) Debug.LogError("Email InputField Image is not assigned!");
        if (passwordInputFieldImage == null) Debug.LogError("Password InputField Image is not assigned!");
        if (guardianPasscodeInputFieldImage == null) Debug.LogError("Guardian Passcode InputField Image is not assigned!");
    }

    public void EnterEditMode()
    {
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;
            Debug.Log($"Edit profile for {userId}");
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }

        SetInputFieldsInteractable(true);
        editButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        saveButton.interactable = true;
        ToggleInputFieldImages(true);
    }

    public void SaveData()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("No user is currently logged in.");
            return;
        }

        string userId = auth.CurrentUser.UserId;

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Failed to retrieve UserId.");
            return;
        }

        string newEmail = emailInputField.text;
        string newPassword = passwordInputField.text;
        string newGuardianPasscode = guardianPasscodeInputField.text;

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "email", newEmail },
            { "password", newPassword },
            { "guardianPasscode", newGuardianPasscode }
        };

        reference.Child("users").Child(userId).UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error saving data: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("Saving data was canceled.");
            }
            else
            {
                Debug.Log("Data saved successfully.");
                saveButton.gameObject.SetActive(false);
                editButton.gameObject.SetActive(true);
                saveButton.interactable = false;
                SetInputFieldsInteractable(false);
                ToggleInputFieldImages(false);
            }
        });
    }

    private void SetInputFieldsInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passwordInputField.interactable = interactable;
        guardianPasscodeInputField.interactable = interactable;
    }

    private void ToggleInputFieldImages(bool visible)
    {
        emailInputFieldImage.enabled = visible;
        passwordInputFieldImage.enabled = visible;
        guardianPasscodeInputFieldImage.enabled = visible;
    }
}
