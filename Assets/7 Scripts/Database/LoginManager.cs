using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField EmailInput;
    public InputField PasswordInput;
    public Text StatusText;

    private FirebaseAuth auth;

    void Start()
    {
        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        PasswordInput.contentType = InputField.ContentType.Password;
    }

    public void LoginUser()
    {
        string email = EmailInput.text.Trim();
        string password = PasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StatusText.text = "Please fill in all fields.";
            Debug.LogWarning("Email or password field is empty.");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Login failed: {task.Exception?.Flatten()}");
                var exception = task.Exception?.Flatten().InnerExceptions[0];
                if (exception != null)
                {
                    HandleLoginError(exception.Message);
                }
            }
            else if (task.IsCompletedSuccessfully)
            {
                // Correct way to access the user from the result
                FirebaseUser user = task.Result.User; 
                Debug.Log($"User logged in successfully: {user.Email}");
                StatusText.text = $"Welcome, {user.Email}!";

		LoadNextScene();
            }
        });
    }

    private void HandleLoginError(string errorMessage)
    {
        Debug.Log($"Error message: {errorMessage}");
        if (errorMessage.Contains("INVALID_EMAIL"))
            StatusText.text = "Invalid email format.";
        else if (errorMessage.Contains("WRONG_PASSWORD"))
            StatusText.text = "Incorrect password.";
        else if (errorMessage.Contains("EMAIL_NOT_FOUND"))
            StatusText.text = "User not found.";
        else
            StatusText.text = "Login failed. Please try again.";
    }

    private void LoadNextScene()
    {
    SceneManager.LoadScene("ModulesPageScene");
    }
}
