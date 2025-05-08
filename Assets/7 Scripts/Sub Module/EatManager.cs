using UnityEngine;

public class EatManager : MonoBehaviour
{
    public Animator mouthAnimator;
    public string triggerName = "PlayMouthAnimation";

    public Animator revealAnimator;
    public string revealTriggerName = "ShowChoices";

    public int totalFoodToEat;
    private int foodEaten = 0;

    void Start()
    {
        // Automatically count all objects tagged as "FoodItem"
        totalFoodToEat = GameObject.FindGameObjectsWithTag("FoodItem").Length;
        Debug.Log("Total food to eat: " + totalFoodToEat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodItem"))
        {
            Debug.Log("Food touched the mouth!");

            if (mouthAnimator != null)
                mouthAnimator.SetTrigger(triggerName);

            Destroy(other.gameObject);
            foodEaten++;

            if (foodEaten >= totalFoodToEat)
            {
                Debug.Log("All food eaten!");

                if (revealAnimator != null)
                    revealAnimator.SetTrigger(revealTriggerName);
            }
        }
    }
}
