using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, IPointerClickHandler
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on " + gameObject.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance?.PlayButtonClickSound();
        AudioInitializer.instance?.PlayButtonClick();
    }
}
