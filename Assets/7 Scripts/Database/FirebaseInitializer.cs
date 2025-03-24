using Firebase;
using Firebase.Extensions;
using UnityEngine;
using System.Threading.Tasks;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance;
    private bool isFirebaseReady = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase is ready!");
                FirebaseApp app = FirebaseApp.DefaultInstance;
                isFirebaseReady = true;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    public bool IsFirebaseReady()
    {
        return isFirebaseReady;
    }
}
