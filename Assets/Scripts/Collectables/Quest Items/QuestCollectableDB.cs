using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Quest Database", menuName = "ScriptableData/Collectables/QuestDatabase")]
public class QuestCollectableDB : ScriptableObject
{
    public QuestCollectableData[] allQuestItems;
    #if UNITY_EDITOR
    [ContextMenu("Get All Quest Items")]
    public void FindAllQuestItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestCollectableData");
        allQuestItems = new QuestCollectableData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            allQuestItems[i] = AssetDatabase.LoadAssetAtPath<QuestCollectableData>(path);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log($"Found {allQuestItems.Length} quest items");
    }
    #endif

    public QuestCollectableData GetItemByName(string name)
    {
        foreach(QuestCollectableData item in allQuestItems)
        {
            if(item.itemName == name)
            {
                return item;
            }
        }
        return null;
    }
}
