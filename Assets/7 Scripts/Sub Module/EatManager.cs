using UnityEngine;

public class EatManager : MonoBehaviour
{
    public Animator mouthAnimator;

    public Animator revealAnimator;
    public string revealTriggerName = "ShowChoices";

    public int totalFoodToEat;
    private int foodEaten = 0;

    public SyncAnimWDialogue syncAnim;

    private bool hasTriggeredFinalStep = false;

    void Start()
    {
        Debug.Log("Total food to eat: " + totalFoodToEat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodItem"))
        {
            Debug.Log("Food touched the mouth!");

            if (syncAnim != null)
            {
                syncAnim.PlayEatMouthAnimation();
            }

            Destroy(other.gameObject);
            foodEaten++;

            if (foodEaten >= totalFoodToEat)
            {
                Debug.Log("All food eaten!");

                if (revealAnimator != null)
                    revealAnimator.SetTrigger(revealTriggerName);
                if (syncAnim != null)
                    syncAnim.PlaySubFinalAudio();
            }
        }
    }

    public void OnAllAudiosFinished()
    {
        if (totalFoodToEat == 0 && !hasTriggeredFinalStep)
        {
            hasTriggeredFinalStep = true; //keep from triggering again

            if (revealAnimator != null)
                revealAnimator.SetTrigger(revealTriggerName);

            if (syncAnim != null)
                syncAnim.PlaySubFinalAudio();
        }
    }
}
