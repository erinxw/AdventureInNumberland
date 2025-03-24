using UnityEngine;
using UnityEngine.SceneManagement; // Only if you're switching scenes

public class BackButton : MonoBehaviour
{
    public void GoBack()
    {
        // Example: Load the previous scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
