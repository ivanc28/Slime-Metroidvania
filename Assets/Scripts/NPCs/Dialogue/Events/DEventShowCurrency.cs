using UnityEngine;

[CreateAssetMenu(fileName = "ShowCurrencyEvent", menuName = "ScriptableData/Dialogue/Event/UI/ShowCurrencyEvent")]
public class DEventShowCurrency : DialogueEvent
{
    public bool showCurrency;
    public override void Invoke()
    {
        UIManager.Instance.DisplayCurrency(showCurrency);
    }
}
