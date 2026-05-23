using UnityEngine;

[CreateAssetMenu(fileName = "HasTool", menuName = "ScriptableData/Dialogue/Conditions/ToolCondition")]
public class HasToolCondition : Condition
{
    public PlayerTools.Tool tool;
    public override bool ConditionMet()
    {
        return base.ConditionMet() && Player.Instance.tools.HasTool(tool);
    }
}
