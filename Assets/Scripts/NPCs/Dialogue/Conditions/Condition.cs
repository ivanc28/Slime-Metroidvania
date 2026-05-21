using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "ScriptableData/Dialogue/Conditions")]
public class Condition : ScriptableObject
{
    public bool ConditionMet()
    {
        return true;
    }
}
