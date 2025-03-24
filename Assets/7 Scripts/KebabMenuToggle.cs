using UnityEngine;

public class KebabMenuHandler : MonoBehaviour
{
    public GameObject dropdownPanel; // The dropdown panel

    private void Start()
    {
        // The listener is added directly in the Inspector, so nothing is needed here
    }

    public void ToggleDropdown()
    {
        dropdownPanel.SetActive(!dropdownPanel.activeSelf);
    }
}
