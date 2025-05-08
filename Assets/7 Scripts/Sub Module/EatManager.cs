using UnityEngine;

public class EatManager : MonoBehaviour
{
    public Animator mouthAnimator; // Drag your mouth animator here
    public string triggerName = "PlayMouthAnimation"; // Match your Animator trigger name

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodItem"))
        {
            Debug.Log("Food touched the mouth!");

            // Play mouth animation
            if (mouthAnimator != null)
            {
                mouthAnimator.SetTrigger(triggerName);
            }

            // Remove the food item
            Destroy(other.gameObject);
        }
    }
}
