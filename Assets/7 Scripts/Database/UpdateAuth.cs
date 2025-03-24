using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;

public class UpdateAuth : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button updateButton;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        updateButton.onClick.AddListener(OnUpdateButtonClicked);
    }

    public void OnUpdateButtonClicked()
    {
        string newEmail = emailInputField.text;
        string newPassword = passwordInputField.text;

        if (string.IsNullOrEmpty(newEmail) || string.IsNullOrEmpty(newPassword))
        {
            Debug.LogError("Email or password is empty.");
            return;
        }

        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            UpdateUserEmail(user, newEmail);
            UpdateUserPassword(user, newPassword);
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }
    }

    private void UpdateUserEmail(FirebaseUser user, string newEmail)
    {
        user.UpdateEmailAsync(newEmail).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Email updated successfully.");
            }
            else
            {
                Debug.LogError("Error updating email: " + task.Exception);
            }
        });
    }

    private void UpdateUserPassword(FirebaseUser user, string newPassword)
    {
        user.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Password updated successfully.");
            }
            else
            {
                Debug.LogError("Error updating password: " + task.Exception);
            }
        });
    }
}
