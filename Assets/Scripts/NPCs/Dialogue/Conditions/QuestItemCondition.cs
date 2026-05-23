using UnityEngine;

[CreateAssetMenu(fileName = "HasQuestCondition", menuName = "ScriptableData/Dialogue/Conditions/HasQuestItem")]
public class QuestItemCondition : Condition
{
    public QuestCollectableData questData;
    public override bool ConditionMet()
    {
        return base.ConditionMet() && Player.Instance.inventory.HasQuestItem(questData);
    }
}
