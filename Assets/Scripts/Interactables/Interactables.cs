using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactables : MonoBehaviour
{
    [SerializeField] string interactableID;
    public InteractableData data;
    [Tooltip("Optional number of pebbles we spawn")]
    public int numPebbles;
    [Tooltip("Optional collectable to spawn")]
    public Collectable collectablePrefab;
    private bool inRange;
    private bool isInteracting;
    [Tooltip("Optional position for where the interaction starts")]
    public Transform interactTransform;
    private Vector2 interactPoint;
    RoomData room;

    private void Awake()
    {
        MakeStart();
    }
    //private void Start()
    //{
    //    //MakeStart();
    //}
    public virtual void MakeStart()
    {
        room = GameManager.Instance.GetCurrRoomData();
        // we check if it was supposed to be destroyed, because otherwise it wouldn't have been added to collectedInteractables
        bool entityFound = room.collectedInteractables.Contains(interactableID) || GameManager.Instance.worldIDs.Contains(interactableID);
        if (entityFound && data.destroyOnInteract)
        {
            BehaviourBeforeDestroy();
            Destroy(gameObject);
        }
        if(interactTransform  != null)
        {
            interactPoint = interactTransform.position;
        }
        else
        {
            interactPoint = transform.position;
        }
    }
    public virtual void BehaviourBeforeDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MakeUpdate();
       
    }
    public virtual void MakeUpdate()
    {
        inRange = Vector2.Distance(interactPoint, Player.Instance.transform.position) <= data.maxDistanceToInteract;
        bool playerCanInteract = (!Player.Instance.InInteraction && !Player.Instance.IsInBubble() && (Player.Instance.GetIsGrounded() || Player.Instance.GetIsLocked()) && !Player.Instance.GetIsAttaching() && !Player.Instance.IsZipping() && !Player.Instance.GetHookBeingThrown()) || Player.Instance.IsOnbubble();
        if (inRange && playerCanInteract && !isInteracting)
        {
            if (Input.GetKeyDown(data.interactKey))
            {
                isInteracting = true;
                StartCoroutine(TryInteract(data.interactionTime));
            }
        }
    }

    private IEnumerator TryInteract(float interactTime)
    {
        yield return new WaitForSeconds(interactTime);       
        if (data.requiredTool.Contains(Player.Instance.tools.GetCurrTool()))
        {
            OnInteract();
            if (data.destroyOnInteract)
            {
                room.collectedInteractables.Add(interactableID);
                GameManager.Instance.worldIDs.Add(interactableID);
                Debug.Log($"adding to colected interactables and destroying {gameObject.name}");
                Destroy(gameObject);
            }
        }
        isInteracting = false;
    }
    public abstract void OnInteract();
    public  void SpawnPebbles(int numPebbles)
    {
        for(int i = 0; i < numPebbles; i++)
        {
            Pebble pebble = Instantiate(data.pebblePrefab, interactPoint + Vector2.up * 0.5f, Quaternion.identity);
            pebble.Initialize(data.pebblePickupDelay);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-data.pebbleLaunchMaxAngle, data.pebbleLaunchMaxAngle), Vector3.forward);
            Vector2 dir = (rotation * Vector2.up).normalized;
            pebble.rb.linearVelocity = dir * (data.pebbleLaunchSpeed + Random.Range(-3f, 1f));
        }
    }

    public void SpawnCollectable()
    {
        if(collectablePrefab != null)
        {
            Collectable collectable = Instantiate(collectablePrefab, interactPoint, Quaternion.identity);
            collectable.Initialize(data.collectablePickupDelay, false);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-data.collectableLaunchMaxAngle, data.collectableLaunchMaxAngle), Vector3.forward);
            Vector2 dir = (rotation * Vector2.up).normalized;
            collectable.rb.linearVelocity = dir * data.collectableLaunchSpeed;
        }

    }
    public RoomData GetRoomOfInteractable()
    {
        return room;
    }
    public string GetInteractableID()
    {
        return interactableID;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 point = Vector2.zero;
        if(interactTransform != null)
        {
            point = interactTransform.position;
        }
        else
        {
            point = transform.position;
        }
            Gizmos.DrawWireSphere(point, data.maxDistanceToInteract);
    }
}
