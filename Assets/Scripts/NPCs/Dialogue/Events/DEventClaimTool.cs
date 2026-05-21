using UnityEngine;

[CreateAssetMenu(fileName = "ClaimToolEvent", menuName = "ScriptableData/Dialogue/Event/ClaimToolEvent")]
public class DEventClaimTool : DialogueEvent
{
    public PlayerTools.Tool claimedTool;
    public override void Invoke()
    {
        Player.Instance.tools.ClaimTool(claimedTool);
    }
}
