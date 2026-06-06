using UnityEngine;
[CreateAssetMenu(fileName = "EitherCondition", menuName = "ScriptableData/Dialogue/Conditions/EitherCondition")]
public class EitherCondition : Condition
{
    public Condition[] conditions;
    public override bool ConditionMet()
    {
        bool result = true;
        foreach (Condition c in conditions)
        {
            result = result && c.ConditionMet();
        }
        return base.ConditionMet() && result;
    }
}
