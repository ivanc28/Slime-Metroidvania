using UnityEngine;

[CreateAssetMenu(fileName = "CollectableData", menuName = "ScriptableData/Collectables/BasicData")]
public class CollectableData : ScriptableObject
{
    public float maxDistanceToCollect;
    [Tooltip("Set the sprite of the OBJECT")]
    public Sprite itemSprite;
    public KeyCode pickupKey;
    public Sprite keySprite;
    public float pickupTime;
    public bool displayMessage = true;
}
