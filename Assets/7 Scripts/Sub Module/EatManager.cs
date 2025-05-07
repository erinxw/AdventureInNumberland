using UnityEngine;

public class EatManager : MonoBehaviour
{
    public Animator mouthAnimator; // Assign in Inspector
    public string triggerName = "PlayMouthAnimation"; // Set this trigger in Animator

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouth")) // Make sure your food items are tagged "Food"
        {
            Debug.Log("Food entered the mouth!");
            mouthAnimator.SetTrigger(triggerName);
            gameObject.SetActive(false);
        }
    }
}
