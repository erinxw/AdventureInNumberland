using UnityEngine;

public class ARSessionController : MonoBehaviour
{
    public GameObject arSession;

    public void StartARSession()
    {
        if (arSession != null)
        {
            arSession.SetActive(true);
        }
    }
}
