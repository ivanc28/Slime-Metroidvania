using UnityEngine;

[CreateAssetMenu(fileName = "HasPurchasedCondition", menuName = "ScriptableData/Dialogue/Conditions/HasPurchasedCondition")]
public class HasPurchasedCondition : Condition
{
    public string purchaseID;
    public override bool ConditionMet()
    {
        return base.ConditionMet() && Player.Instance.inventory.HasPurchased(purchaseID);
    }

}
