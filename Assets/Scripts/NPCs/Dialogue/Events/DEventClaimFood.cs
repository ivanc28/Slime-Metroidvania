using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ClaimFoodEvent", menuName = "ScriptableData/Dialogue/Event/ClaimFoodEvent")]
public class DEventClaimFood : DialogueEvent
{
    public enum Type { Food, Drink }
    public Type consumableType;
    public string claimMessage;
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
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        Invoke();
        yield return runner.StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(claimMessage));
    }
}
