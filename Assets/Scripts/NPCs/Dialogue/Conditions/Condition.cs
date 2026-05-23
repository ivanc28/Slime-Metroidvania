using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "ScriptableData/Dialogue/Conditions/NoConditions")]
public class Condition : ScriptableObject
{
    public virtual bool ConditionMet()
    {
        return true;
    }
}
