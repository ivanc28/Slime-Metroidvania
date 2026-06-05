using UnityEngine;

[CreateAssetMenu(fileName = "giveItemEvent", menuName = "ScriptableData/Dialogue/Event/GiveItem")]
public class DEventGiveItem : DialogueEvent
{
    public QuestCollectableData[] questItems;
    public override void Invoke()
    {
        foreach (QuestCollectableData item  in questItems)
        {
            Player.Instance.inventory.RemoveQuestItem(item);
        }
    }
}
