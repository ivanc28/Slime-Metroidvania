using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private List<QuestCollectableData> questItemList;
    public void AddQuestItem(QuestCollectableData item)
    {
        questItemList.Add(item);
    }
    public bool HasQuestItem(QuestCollectableData item)
    {
        return questItemList.Contains(item);
    }
}
