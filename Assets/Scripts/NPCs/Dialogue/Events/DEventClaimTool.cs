using UnityEngine;

[CreateAssetMenu(fileName = "ClaimToolEvent", menuName = "ScriptableData/Dialogue/Event/ClaimToolEvent")]
public class DEventClaimTool : DialogueEvent
{
    public PlayerTools.Tool claimedTool;
    public int optionalPebbleCost = 0;
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
        Debug.Log("Tried to claim a tool via dialogue");
    }
}
