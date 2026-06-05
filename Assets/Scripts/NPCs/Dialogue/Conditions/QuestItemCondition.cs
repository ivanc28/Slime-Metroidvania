using UnityEngine;

[CreateAssetMenu(fileName = "HasQuestCondition", menuName = "ScriptableData/Dialogue/Conditions/HasQuestItem")]
public class QuestItemCondition : Condition
{
    public QuestCollectableData[] questData;
    public override bool ConditionMet()
    {
        bool hasAllItems = true;
        foreach(QuestCollectableData item in questData)
        {
            hasAllItems = hasAllItems && Player.Instance.inventory.HasQuestItem(item);
        }
        return base.ConditionMet() && hasAllItems;
    }
}
