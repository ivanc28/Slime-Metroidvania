using UnityEngine;

[CreateAssetMenu(fileName = "giveItemEvent", menuName = "ScriptableData/Dialogue/Event/GiveItem")]
public class DEventGiveItem : DialogueEvent
{
    public QuestCollectableData questItem;
    public override void Invoke()
    {
        Player.Instance.inventory.RemoveQuestItem(questItem);
    }
}
