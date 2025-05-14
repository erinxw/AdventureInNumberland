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

    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    void Start()
    {
        // Initialize Firebase Authentication
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser()
    {
        if (string.IsNullOrEmpty(Email.text) ||
            string.IsNullOrEmpty(Password.text) || string.IsNullOrEmpty(GuardianPasscode.text))
        {
            Debug.Log("All fields are required.");
            return;
        }

        // Validate the email format
        if (!IsValidEmail(Email.text))
        {
            Debug.LogError("Invalid email format.");
            return;
        }

        if (!IsValidPassword(Password.text))
        {
            Debug.LogError("Password must be at least 8 characters long and include uppercase letters, lowercase letters, numbers, and special characters.");
            return;
        }

        // Create user with Firebase Authentication
        auth.CreateUserWithEmailAndPasswordAsync(Email.text, Password.text).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle error by using FirebaseException from the correct namespace
                FirebaseException exception = task.Exception?.Flatten().InnerExceptions[0] as FirebaseException;
                if (exception != null)
                {
                    Debug.LogError("Error creating user: " + exception.Message);
                }
            }
            else if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                Debug.Log($"User created successfully: {newUser.Email}");

                // Create a User object and store it in the Firebase Realtime Database
                User newUserObject = new User(Email.text, int.Parse(GuardianPasscode.text));
                StoreUserDataInDatabase(newUser.UserId, newUserObject);
		
		LoadNextScene();
            }
        });
    }

    // Store user data in Firebase Realtime Database
    private void StoreUserDataInDatabase(string userId, User newUser)
    {
        // Convert the user object to a JSON string
        string json = JsonUtility.ToJson(newUser);

        // Store the user profile in the database under the user's UID
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

    // Email validation using regex
    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsValidPassword(string password)
    {
        // Ensure the password has at least 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 special character
        string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+[\]{};:'""\\|,.<>?/`~])[A-Za-z\d!@#$%^&*()\-_=+[\]{};:'""\\|,.<>?/`~]{8,}$";
        return Regex.IsMatch(password, passwordPattern);
    }

    private void LoadNextScene()
    {
    SceneManager.LoadScene("IntroductionPageScene");
    }
}
