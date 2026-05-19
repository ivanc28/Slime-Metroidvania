using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "ScriptableData/Interactable")]
public class InteractableData : ScriptableObject
{
    public KeyCode interactKey;
    public PlayerTools.Tool requiredTool;
    public Pebble pebblePrefab;
    public float pebblePickupDelay;
    public float pebbleLaunchSpeed;
    public float pebbleLaunchMaxAngle;

}
