using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ClaimFoodEvent", menuName = "ScriptableData/Dialogue/Event/ClaimFoodEvent")]
public class DEventClaimFood : DialogueEvent
{
    public enum Type { Food, Drink }
    public Type consumableType;
    public string claimMessage;
    public Sprite itemSprite;
    public override void Invoke()
    {
        ClaimFood();
    }
    private void ClaimFood()
    {
        if (consumableType == Type.Food)
        {
            Player.Instance.IncrementGrappleCharges();
        }
        else
        {
            Player.Instance.IncreaseGrappleLength();
        }
        Player.Instance.SetFoodHolder(itemSprite);
        Player.Instance.anim.SetTrigger("eat");
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        Invoke();
        yield return runner.StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(claimMessage));
    }
}
