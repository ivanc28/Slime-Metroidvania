using UnityEngine;

[CreateAssetMenu(fileName = "EnoughCurrency", menuName = "ScriptableData/Dialogue/Conditions/CurrencyCondition")]
public class EnoughCurrencyCondition : Condition
{
    public int minCurrencyRequired;
    public override bool ConditionMet()
    {
        return base.ConditionMet() && Player.Instance.currencyData.GetCurrency() >= minCurrencyRequired;
    }
}
