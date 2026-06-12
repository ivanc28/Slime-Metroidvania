using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private List<QuestCollectableData> questItemList = new();
    private List<ToolInventoryData> toolItemList = new();
    private HashSet<string> completedPurchases = new();
    private HashSet<GameManager.Region> collectedMaps = new();
    public void AddQuestItem(QuestCollectableData item)
    {
        questItemList.Add(item);
        //Debug.Log($"Items in inventory: {questItemList.Count}");
    }
    public void RemoveQuestItem(QuestCollectableData item)
    {
        questItemList.Remove(item);
        //Debug.Log($"Items in inventory: {questItemList.Count}");
    }
    public bool HasQuestItem(QuestCollectableData item)
    {
        return questItemList.Contains(item);
    }
    public void AddToolItem(ToolInventoryData toolItem)
    {
        if (!toolItemList.Contains(toolItem))
        {
            toolItemList.Add(toolItem);
        }
    }
    public List<QuestCollectableData> GetAllItems()
    {
        return questItemList;
    }
    public List<ToolInventoryData> GetAllTools()
    {
        return toolItemList;
    }
    public void CompletePurchase(string purchaseID)
    {
        completedPurchases.Add(purchaseID);
    }

    public bool HasPurchased(string purchaseID)
    {
        return completedPurchases.Contains(purchaseID);
    }

    public void CollectMap(GameManager.Region regionMap)
    {
        collectedMaps.Add(regionMap);
    }
    public bool HasMap(GameManager.Region regionMap)
    {
        return collectedMaps.Contains(regionMap);
    }
}
