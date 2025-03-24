using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPageController : MonoBehaviour
{
    public void OnLoginButtonPressed()
    {
        Debug.Log("Login Button Pressed");
        // Add navigation logic here
        SceneManager.LoadScene("LoginScene");
    }

    public void OnSignupButtonPressed()
    {
        Debug.Log("Signup Button Pressed");
        // Add navigation logic here
        SceneManager.LoadScene("SignupScene");
    }
}
