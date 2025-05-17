using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class ProfileManager : MonoBehaviour
{
    public InputField emailInputField;
    public InputField guardianPasscodeInputField;
    public Text StatusText;              // Reference to the error message text UI (optional)
    public GameObject StatusGroup;

    private DatabaseReference reference;
    private FirebaseAuth auth;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
                return;
            }

            auth = FirebaseAuth.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            RetrieveDataForLoggedInUser();
        });

        // Hide the error message initially
        if (StatusGroup != null)
            StatusGroup.SetActive(false);
    }

    public void RetrieveDataForLoggedInUser()
    {
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;

            FirebaseDatabase.DefaultInstance
                .RootReference.Child("users").Child(userId)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        ShowStatus("Error fetching data: " + task.Exception);
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if (snapshot.Exists)
                        {
                            string email = snapshot.Child("email").Value?.ToString();
                            string guardianPasscode = snapshot.Child("guardianPasscode").Value?.ToString();

                            emailInputField.text = email;
                            guardianPasscodeInputField.text = guardianPasscode;

                            // Make the input fields non-editable initially (view mode)
                            SetFieldsInteractable(false);
                        }
                        else
                        {
                            ShowStatus("Profile not found. Please log in again.");
                        }
                    }
                });
        }
        else
        {
            ShowStatus("Please log in to see your profile.");
        }
    }

    private void SetFieldsInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        guardianPasscodeInputField.interactable = interactable;
    }

    private void ShowStatus(string message)
    {
        if (StatusGroup != null)
            StatusGroup.SetActive(true);

        if (StatusText != null)
            StatusText.text = message;
    }
}
