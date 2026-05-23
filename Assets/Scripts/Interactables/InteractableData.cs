using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "ScriptableData/Interactable")]
public class InteractableData : ScriptableObject
{
    public KeyCode interactKey;
    public PlayerTools.Tool[] requiredTool;
    public bool destroyOnInteract;
    public float maxDistanceToInteract;
    public float interactionTime;
    [Header("Optional Pebble Spawning Data")]
    public Pebble pebblePrefab;
    public float pebblePickupDelay;
    public float pebbleLaunchSpeed;
    public float pebbleLaunchMaxAngle;
    [Header("Optional Collectable Spawning Data")]
    public Collectable collectablePrefab;
    public float collectablePickupDelay;
    public float collectableLaunchSpeed;
    public float collectableLaunchMaxAngle;

}
