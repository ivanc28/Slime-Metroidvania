using UnityEngine;

[CreateAssetMenu(fileName = "CollectableData", menuName = "ScriptableData/Collectable")]
public class CollectableData : ScriptableObject
{
    public float maxDistanceToCollect;
    public Sprite itemSprite;
    public KeyCode pickupKey;
    public Sprite keySprite;
    public float pickupTime;
}
