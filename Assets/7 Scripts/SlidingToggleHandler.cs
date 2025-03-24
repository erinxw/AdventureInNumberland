using UnityEngine;
using UnityEngine.UI;

public class SlidingToggleHandler : MonoBehaviour
{
    public Toggle toggle;
    public Image slidingKnob; // The knob that slides
    public Transform knobOnPosition; // Position when the toggle is on
    public Transform knobOffPosition; // Position when the toggle is off
    public Image background; // The background image
    public Sprite onBackground; // Background image for the "On" state
    public Sprite offBackground; // Background image for the "Off" state

    private void Start()
    {
        // Set the initial state based on the toggle's value
        UpdateToggle(toggle.isOn);

        // Add listener to handle the toggle switch
        toggle.onValueChanged.AddListener(UpdateToggle);
    }

    private void UpdateToggle(bool isOn)
    {
        // Move the knob to the appropriate position
        slidingKnob.rectTransform.position = isOn ? knobOnPosition.position : knobOffPosition.position;

        // Change the background based on the toggle state
        background.sprite = isOn ? onBackground : offBackground;
    }
}
