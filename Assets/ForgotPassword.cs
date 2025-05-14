using Firebase;
using Firebase.Auth;
using UnityEngine;
using Firebase.Extensions;
using UnityEngine.UI;  // For InputField and Button

public class ForgotPassword : MonoBehaviour
{
    // Firebase Authentication
    private FirebaseAuth auth;

    // UI elements
    public InputField emailField;      // Email input field
    public Text feedbackText;          // Feedback display
    public Button resetButton;         // PUBLIC RESET BUTTON

    void Start()
    {
        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        // Optional: Link the button to the function, if not set in Inspector
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(SendPasswordResetEmail);
        }
    }

    // Method to send password reset email
    public void SendPasswordResetEmail()
    {
        string email = emailField.text;

        if (!string.IsNullOrEmpty(email))
        {
            auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    feedbackText.text = "Error: " + task.Exception?.Flatten().InnerException?.Message;
                }
                else
                {
                    feedbackText.text = "Password reset email sent! Check your inbox.";
                }
            });
        }
        else
        {
            feedbackText.text = "Please enter your email address.";
        }
    }
}
