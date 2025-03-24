using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class ProfileManager : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField guardianPasscodeInputField;

    private DatabaseReference reference;
    private FirebaseAuth auth;

    private void Start()
    {
        passwordInputField.contentType = InputField.ContentType.Password;

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
                        Debug.LogError("Error fetching data: " + task.Exception);
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if (snapshot.Exists)
                        {
                            string email = snapshot.Child("email").Value?.ToString();
                            string password = snapshot.Child("password").Value?.ToString();
                            string guardianPasscode = snapshot.Child("guardianPasscode").Value?.ToString();

                            emailInputField.text = email;
                            passwordInputField.text = password;
                            guardianPasscodeInputField.text = guardianPasscode;

                            // Make the input fields non-editable initially (view mode)
                            SetFieldsInteractable(false);
                        }
                        else
                        {
                            Debug.Log("No data available for the user.");
                        }
                    }
                });
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }
    }

    private void SetFieldsInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passwordInputField.interactable = interactable;
        guardianPasscodeInputField.interactable = interactable;
    }
}
