using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // To load the profile scene
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;

public class FirebasePasscodeManager : MonoBehaviour
{
    private DatabaseReference databaseReference; // Firebase database reference
    private FirebaseAuth firebaseAuth;  // Firebase authentication reference
    public InputField passcodeInputField;  // Reference to the input field for entering the passcode
    public Button submitButton;            // Reference to the submit button
    public Text errorMessage;              // Reference to the error message text UI (optional)

    private string correctPasscode;

    private void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            firebaseAuth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.GetInstance(app).RootReference;

            // Check if user is logged in and fetch the passcode
            if (firebaseAuth.CurrentUser != null)
            {
                FetchPasscodeFromFirebase(firebaseAuth.CurrentUser.UserId);
            }
            else
            {
                Debug.LogError("User is not logged in.");
            }
        });

        // Add listener for the submit button
        submitButton.onClick.AddListener(CheckPasscode);

        // Optionally hide the error message initially
        if (errorMessage != null)
        {
            errorMessage.gameObject.SetActive(false);
        }
    }

    // Fetch passcode for the logged-in user
    private void FetchPasscodeFromFirebase(string userId)
    {
        // Fetch the passcode for the logged-in user
        databaseReference.Child("users").Child(userId).Child("guardianPasscode").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve passcode from Firebase");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    correctPasscode = snapshot.Value.ToString(); // Get the passcode value from Firebase
                    Debug.Log("Passcode fetched from Firebase: " + correctPasscode);
                }
                else
                {
                    Debug.LogError("No passcode found for this user.");
                }
            }
        });
    }

    // Function to check if the entered passcode is correct
    private void CheckPasscode()
    {
        string enteredPasscode = passcodeInputField.text;

        // Check if the passcode is fetched from Firebase
        if (string.IsNullOrEmpty(correctPasscode))
        {
            Debug.LogError("Passcode not fetched from Firebase yet.");
            return;
        }

        // Compare entered passcode with the correct passcode
        if (enteredPasscode == correctPasscode)
        {
            // Correct passcode, load the profile page scene
            Debug.Log("Passcode correct, accessing profile page!");
            SceneManager.LoadScene("ProfilePageScene"); // Change "ProfileScene" to the name of your profile scene
        }
        else
        {
            // Incorrect passcode, show error message
            Debug.Log("Incorrect passcode!");
            if (errorMessage != null)
            {
                errorMessage.gameObject.SetActive(true); // Show the error message
                errorMessage.text = "Incorrect passcode, please try again.";
            }
        }
    }
}
