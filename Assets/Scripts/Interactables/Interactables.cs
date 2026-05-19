using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    [SerializeField] bool destroyOnInteract;
    [SerializeField] string interactableID;
    [SerializeField] InteractableData data;
    private bool inRange;

    RoomData room;

    private void Start()
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
        if (inRange)
        {
            if (Input.GetKeyDown(data.interactKey))
            {
                if (Player.Instance.tools.GetCurrTool() == data.requiredTool)
                {
                    OnInteract();
                    if (destroyOnInteract)
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
