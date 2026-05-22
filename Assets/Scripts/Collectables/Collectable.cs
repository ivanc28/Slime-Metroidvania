using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public CollectableData data;
    private bool inRange;
    private void Update()
    {
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= data.maxDistanceToCollect;
        if (inRange)
        {
            if (Input.GetKeyDown(data.pickupKey) && CanCollect())
            {
                Collect();
            }
        }
    }
    public virtual bool CanCollect()
    {
        return true;
    }
    public abstract void Collect();
    
}
