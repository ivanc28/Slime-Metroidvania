using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Quest Database", menuName = "ScriptableData/Collectables/QuestDatabase")]
public class QuestCollectableDB : ScriptableObject
{
    public QuestCollectableData[] allQuestItems;
}
