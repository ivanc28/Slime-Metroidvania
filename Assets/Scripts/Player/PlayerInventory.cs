using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private List<QuestCollectableData> questItemList = new();
    public void AddQuestItem(QuestCollectableData item)
    {
        questItemList.Add(item);
        Debug.Log($"Items in inventory: {questItemList.Count}");
    }
    public void RemoveQuestItem(QuestCollectableData item)
    {
        questItemList.Remove(item);
        Debug.Log($"Items in inventory: {questItemList.Count}");
    }
    public bool HasQuestItem(QuestCollectableData item)
    {
        return questItemList.Contains(item);
    }
}
