using UnityEngine;

[CreateAssetMenu(fileName = "NeverPassCondition", menuName = "ScriptableData/Dialogue/Conditions/NeverPass")]
public class NeverPassCondition : Condition
{
    public override bool ConditionMet()
    {
        return base.ConditionMet() && false;
    }
}
