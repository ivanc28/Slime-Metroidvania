using UnityEngine;
[CreateAssetMenu(fileName = "MultiCondition", menuName = "ScriptableData/Dialogue/Conditions/MultiCondition")]
public class MultiCondition : Condition
{
    public Condition[] conditions;
    [Tooltip("If checked, we see if ALL conditions met. If not checked, we see if ANY condition is met")]
    public bool and;
    public override bool ConditionMet()
    {
        bool result = false;
        if (and)
        {
            result = true;
            foreach (Condition c in conditions)
            {
                if (!c.ConditionMet())
                {
                    result = false;
                    break;
                }
            }
        }
        else
        {
            foreach (Condition c in conditions)
            {
                if (c.ConditionMet())
                {
                    result = true;
                    break;
                }
            }
        }
        return base.ConditionMet() && result;
    }
}
