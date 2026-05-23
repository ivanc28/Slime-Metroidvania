using UnityEngine;

public class QuestCollectable : Collectable
{
    QuestCollectableData questData;
    public override void MakeStart()
    {
        base.MakeStart();
        questData = data as QuestCollectableData;
        if (questData == null)
        {
            Debug.LogError("data is not a QuestCollectableData on " + gameObject.name);
        }
    }
    public override void Collect()
    {
        Player.Instance.inventory.AddQuestItem(questData);
    }
}
