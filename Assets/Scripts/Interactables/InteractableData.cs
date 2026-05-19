using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "ScriptableData/Interactable")]
public class InteractableData : ScriptableObject
{
    public KeyCode interactKey;
    public PlayerTools.Tool requiredTool;
    public GameObject pebblePrefab;
}
