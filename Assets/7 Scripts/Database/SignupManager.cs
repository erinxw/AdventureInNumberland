using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Firebase.Database;
using Firebase;
using UnityEngine.SceneManagement;

public class SignupManager : MonoBehaviour
{
    public InputField Email;
    public InputField Password;
    public InputField GuardianPasscode;
    public GameObject StatusGroup;
    public Text StatusText;

    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (StatusGroup != null)
            StatusGroup.SetActive(false);
    }

    public void CreateUser()
    {
        // Trim input fields
        string email = Email.text.Trim();
        string password = Password.text.Trim();
        string passcode = GuardianPasscode.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passcode))
        {
            Debug.Log("All fields are required.");
            ShowStatus("All fields are required.");
            return;
        }

        if (!IsValidEmail(email))
        {
            Debug.LogError("Invalid email format.");
            ShowStatus("Invalid email format.");
            return;
        }

        if (!IsValidPassword(password))
        {
            Debug.LogError("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.");
            ShowStatus("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.");
            return;
        }

        if (!IsValidGuardianPasscode(passcode))
        {
            Debug.LogError("Guardian passcode must be exactly 4 digits.");
            ShowStatus("Guardian passcode must be exactly 4 digits.");
            return;
        }

        // Create user
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                FirebaseException exception = task.Exception?.Flatten().InnerExceptions[0] as FirebaseException;
                if (exception != null)
                {
                    Debug.LogError("Error creating user: " + exception.Message);
                    ShowStatus("Error creating user: " + exception.Message);
                }
            }
            else if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                Debug.Log($"User created successfully: {newUser.Email}");
                ShowStatus("User created successfully!");

                User newUserObject = new User(email, int.Parse(passcode));
                StoreUserDataInDatabase(newUser.UserId, newUserObject);

                LoadNextScene();
            }
        });
    }

    private void StoreUserDataInDatabase(string userId, User newUser)
    {
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error storing user data in database: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("User data stored successfully in database!");
                }
            });
    }

    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsValidPassword(string password)
    {
        string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+\[\]{};:'""\\|,.<>?/`~])[A-Za-z\d!@#$%^&*()\-_=+\[\]{};:'""\\|,.<>?/`~]{8,}$";
        return Regex.IsMatch(password, passwordPattern);
    }

    private bool IsValidGuardianPasscode(string passcode)
    {
        return Regex.IsMatch(passcode, @"^\d{4}$");
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("IntroductionPageScene");
    }

    private void ShowStatus(string message)
    {
        if (StatusGroup != null)
            StatusGroup.SetActive(true);

        if (StatusText != null)
            StatusText.text = message;
    }
}
