using System.Collections;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public CollectableData data;
    public string collectableID;
    private bool inRange;

    RoomData room;
    private void Start()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.collectedCollectables.Contains(collectableID))
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= data.maxDistanceToCollect;
        if (inRange)
        {
            if (Input.GetKeyDown(data.pickupKey) && CanCollect())
            {
                StartCoroutine(PickUp());
            }
        }
    }
    public virtual bool CanCollect()
    {
        return !Player.Instance.IsInBubble() && !Player.Instance.InInteraction;
    }
    public abstract void Collect();
    private IEnumerator PickUp()
    {
        yield return new WaitForSeconds(data.pickupTime);
        Collect();
        room.collectedCollectables.Add(collectableID);
    }
    
}
