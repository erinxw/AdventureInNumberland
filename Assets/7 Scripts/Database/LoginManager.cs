using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

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

        // Hide status group at the beginning
        if (StatusGroup != null)
            StatusGroup.SetActive(false);
    }

    public void LoginUser()
    {
        string email = EmailInput.text.Trim();
        string password = PasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please fill in all fields.");
            Debug.LogWarning("Email or password field is empty.");
            return;
        }

        LoginButton.interactable = false;
        ShowStatus("Logging in...");

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            LoginButton.interactable = true;

            if (task.IsFaulted)
            {
                Debug.LogError($"Login failed: {task.Exception?.Flatten()}");
                var exception = task.Exception?.Flatten().InnerExceptions[0];

                if (exception is Firebase.FirebaseException firebaseEx)
                {
                    HandleLoginError((AuthError)firebaseEx.ErrorCode);
                }
                else
                {
                    ShowStatus("Login failed. Please try again.");
                }
            }
            else if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log($"User logged in successfully: {user.Email}");
                ShowStatus($"Welcome, {user.Email}!");
                LoadNextScene();
            }
        });
    }

    private void HandleLoginError(AuthError errorCode)
    {
        Debug.Log($"Firebase Auth Error Code: {errorCode}");

        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                ShowStatus("Invalid email format.");
                break;
            case AuthError.WrongPassword:
                ShowStatus("Incorrect password.");
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
