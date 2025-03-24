using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public InputField passwordInputField;  // Reference to the password InputField
    public Button toggleButton;  // Reference to the toggle button
    public Image toggleButtonImage;  // Reference to the Image component of the toggle button
    public Sprite showPasswordIcon;  // Icon for showing password
    public Sprite hidePasswordIcon;  // Icon for hiding password

    private bool isPasswordVisible = false;  // Flag to track password visibility

    private void Start()
    {
        // Set the initial icon
        toggleButtonImage.sprite = hidePasswordIcon;

        // Ensure the button click calls TogglePasswordVisibility method
        toggleButton.onClick.AddListener(TogglePasswordVisibility);
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;  // Toggle the flag

        // Set the content type and icon based on the visibility flag
        if (isPasswordVisible)
        {
            passwordInputField.contentType = InputField.ContentType.Standard;  // Show text
            toggleButtonImage.sprite = showPasswordIcon;  // Set icon to show password
        }
        else
        {
            passwordInputField.contentType = InputField.ContentType.Password;  // Hide text
            toggleButtonImage.sprite = hidePasswordIcon;  // Set icon to hide password
        }

        // Apply the change immediately
        passwordInputField.ForceLabelUpdate();
    }
}
