using UnityEngine;

public class QuestCollectable : Collectable
{
    QuestCollectableData questData;
    public override void MakeStart()
    {
        base.MakeStart();
        questData = data as QuestCollectableData;
    }
    public override void Collect()
    {
        Player.Instance.inventory.AddQuestItem(questData);
    }
}
