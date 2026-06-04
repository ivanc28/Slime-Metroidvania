using UnityEngine;

[CreateAssetMenu(fileName = "EnoughGrapplesCondition", menuName = "ScriptableData/Dialogue/Conditions/EnoughGrapples")]
public class EnoughGrapplesCondition : Condition
{
    public int minGrapplesRequired;
    public override bool ConditionMet()
    {
        return base.ConditionMet() && Player.Instance.maxGrappleCharges >= minGrapplesRequired;
    }
}
