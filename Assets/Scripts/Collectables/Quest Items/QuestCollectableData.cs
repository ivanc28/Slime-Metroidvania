using UnityEngine;

[CreateAssetMenu(fileName = "QuestCollectableData", menuName = "ScriptableData/Collectables/QuestCollectable")]
public class QuestCollectableData : CollectableData
{
    [Header("Quest Data")]
    public string itemName;
    public string description;
    [Tooltip("Set the sprite of the UI ELEMENT")]
    public Sprite sprite;
}
