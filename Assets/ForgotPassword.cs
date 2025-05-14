using Firebase;
using Firebase.Auth;
using UnityEngine;
using Firebase.Extensions;
using UnityEngine.UI;

public class ForgotPassword : MonoBehaviour
{
    // Firebase Authentication
    private FirebaseAuth auth;

    // UI elements
    public InputField emailField;      // Email input field
    public Text StatusText;
    public GameObject StatusGroup;
    public Button resetButton;         // PUBLIC RESET BUTTON

    void Start()
    {
        // Hide status group at the beginning
        if (StatusGroup != null)
            StatusGroup.SetActive(false);

        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        if (auth != null)
        {
            Debug.Log("FirebaseAuth initialized successfully.");
        }

        // Optional: Link the button to the function, if not set in Inspector
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(SendPasswordResetEmail);
            Debug.Log("Reset button listener added.");
        }
        else
        {
            Debug.LogWarning("Reset button is not assigned in Inspector!");
        }
    }

    // Method to send password reset email
    public void SendPasswordResetEmail()
    {
        string email = emailField.text.Trim().Replace(" ","");
        Debug.Log("Attempting to send password reset to: " + email);

        if (!string.IsNullOrEmpty(email))
        {
            auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    ShowStatus("Request canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    ShowStatus("Error: " + task.Exception?.Flatten().InnerException?.Message);
                }
                else if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Password reset email sent successfully.");
                    ShowStatus("Password reset email sent! Check your inbox.");
                }
            });
        }
        else
        {
            Debug.LogWarning("Email field is empty.");
            ShowStatus("Please enter your email address.");
        }
    }

    private void ShowStatus(string message)
    {
        if (StatusGroup != null)
            StatusGroup.SetActive(true);

        if (StatusText != null)
            StatusText.text = message;
    }
}
