using UnityEngine;

[CreateAssetMenu(fileName = "ToolInventoryData", menuName = "ScriptableData/UI/ToolInventoryData")]
public class ToolInventoryData : ScriptableObject
{
    public PlayerTools.Tool tool;
    public string toolName;
    public string description;
    public Sprite sprite;
}
