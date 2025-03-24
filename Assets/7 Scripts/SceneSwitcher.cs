using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject progressBar;

    public void LoadSceneByName(string sceneName)
    {
        if (sceneName == "ModulesPageScene" && progressBar != null)
        {
            Destroy(progressBar);
        }
        SceneManager.LoadScene(sceneName);
    }
}
