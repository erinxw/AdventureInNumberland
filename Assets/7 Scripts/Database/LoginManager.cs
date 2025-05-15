using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoginManager : MonoBehaviour
{
    public InputField EmailInput;
    public InputField PasswordInput;
    public Text StatusText;
    public Button LoginButton;
    public GameObject StatusGroup;  // Group containing background, icon, and StatusText

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        PasswordInput.contentType = InputField.ContentType.Password;

        if (StatusGroup != null)
            StatusGroup.SetActive(false);  // Hide status UI initially
    }

    public void LoginUser()
    {
        string email = EmailInput.text.Trim();
        string password = PasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please fill in all fields.");
            Debug.LogWarning("Email or password is empty.");
            return;
        }

        LoginButton.interactable = false;
        ShowStatus("Logging in...");

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            LoginButton.interactable = true;

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Login failed: " + task.Exception?.Flatten().Message);
                bool handled = false;

                foreach (var e in task.Exception.Flatten().InnerExceptions)
                {
                    if (e is Firebase.FirebaseException firebaseEx)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogError("Firebase auth error: " + errorCode);
                        HandleLoginError(errorCode);
                        handled = true;
                        break;
                    }
                }

                if (!handled)
                {
                    Debug.LogWarning("Unhandled login error: not a FirebaseException.");
                    ShowStatus("Something went wrong. Please try again.");
                }
            }
            else if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log($"User logged in: {user.Email}");
                ShowStatus($"Welcome, {user.Email}!");
                LoadNextScene();
            }
        });
    }

    private void HandleLoginError(AuthError errorCode)
    {
        Debug.Log($"Firebase Auth Error Code: {errorCode} ({(int)errorCode})");

        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                ShowStatus("Invalid email format.");
                break;
            case AuthError.WrongPassword:
            case AuthError.InvalidCredential:
                ShowStatus("Email or password is incorrect.");
                break;
            case AuthError.UserNotFound:
                ShowStatus("User not found.");
                break;
            case AuthError.MissingEmail:
                ShowStatus("Email is required.");
                break;
            case AuthError.MissingPassword:
                ShowStatus("Password is required.");
                break;
            case AuthError.TooManyRequests:
                ShowStatus("Too many attempts. Please try again later.");
                break;
            case AuthError.NetworkRequestFailed:
                ShowStatus("Network error. Please check your internet connection.");
                break;
            default:
                ShowStatus("Login failed. Please try again.");
                break;
        }
    }

    private void ShowStatus(string message)
    {
        if (StatusGroup != null)
            StatusGroup.SetActive(true);

        if (StatusText != null)
            StatusText.text = message;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("ModulesPageScene");
    }
}
