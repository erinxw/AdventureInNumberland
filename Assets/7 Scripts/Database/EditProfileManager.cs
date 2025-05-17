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
    public InputField guardianPasscodeInputField;

    public Button editButton;
    public Button saveButton;

    public Image emailInputFieldImage;
    public Image guardianPasscodeInputFieldImage;

    private FirebaseAuth auth;
    private DatabaseReference reference;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                saveButton.interactable = false;
                SetInputFieldsInteractable(false);

                editButton.onClick.AddListener(EnterEditMode);
                saveButton.onClick.AddListener(SaveData);
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
        });
    }

    public void EnterEditMode()
    {
        if (auth == null || auth.CurrentUser == null || editButton == null || saveButton == null)
        {
            Debug.LogError("Cannot enter edit mode: Firebase auth or buttons not properly initialized.");
            return;
        }

        SetInputFieldsInteractable(true);
        editButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        saveButton.interactable = true;
        ToggleInputFieldImages(true);
    }

    public void SaveData()
    {
        if (auth == null || auth.CurrentUser == null)
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
        string newGuardianPasscode = guardianPasscodeInputField.text;

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "email", newEmail },
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
        guardianPasscodeInputField.interactable = interactable;
    }

    private void ToggleInputFieldImages(bool visible)
    {
        emailInputFieldImage.enabled = visible;
        guardianPasscodeInputFieldImage.enabled = visible;
    }
}
