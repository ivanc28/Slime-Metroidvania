using System.Linq;
using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    [SerializeField] string interactableID;
    public InteractableData data;
    public int numPebbles;
    private bool inRange;

    RoomData room;

    private void Start()
    {
        MakeStart();
    }
    public virtual void MakeStart()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.collectedInteractables.Contains(interactableID))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= data.maxDistanceToInteract;

        if (inRange)
        {
            if (Input.GetKeyDown(data.interactKey))
            {
                if (data.requiredTool.Contains(Player.Instance.tools.GetCurrTool()))
                {
                    OnInteract();
                    if (data.destroyOnInteract)
                    {
                        room.collectedInteractables.Add(interactableID);
                        Destroy(gameObject);
                    }
                }
            }
        }    
       
    }
    public abstract void OnInteract();
    public  void SpawnPebbles(int numPebbles)
    {
        for(int i = 0; i < numPebbles; i++)
        {
            Pebble pebble = Instantiate(data.pebblePrefab, transform.position, Quaternion.identity);
            pebble.Initialize(data.pebblePickupDelay);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-data.pebbleLaunchMaxAngle, data.pebbleLaunchMaxAngle), Vector3.forward);
            Vector2 dir = (rotation * Vector2.up).normalized;
            pebble.rb.linearVelocity = dir * data.pebbleLaunchSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.maxDistanceToInteract);
    }
}
