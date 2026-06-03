using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ClaimToolEvent", menuName = "ScriptableData/Dialogue/Event/ClaimToolEvent")]
public class DEventClaimTool : DialogueEvent
{
    public PlayerTools.Tool claimedTool;
    public int optionalPebbleCost = 0;
    public string claimMessage;
    public override void Invoke()
    {
        if(optionalPebbleCost > 0)
        {
            if (Player.Instance.currencyData.OnPurchase(optionalPebbleCost))
            {
                Player.Instance.tools.ClaimTool(claimedTool);
            }
        }
        else
        {
            Player.Instance.tools.ClaimTool(claimedTool);
        }
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        Invoke();
        yield return runner.StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(claimMessage));
    }
}
