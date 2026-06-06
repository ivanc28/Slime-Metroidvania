using System.Collections;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public CollectableData data;
    public string collectableID;
    [Tooltip("The message should fit one line")]
    public string pickupMessage;
    public GameObject keyIcon;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;
    private bool inRange;
    private float pickupTime;
    RoomData room;
    private bool checkID = true;
    private void Awake()
    {
        MakeStart();
    }
    public virtual void MakeStart()
    {
        room = GameManager.Instance.GetCurrRoomData();
        bool entityFound = room.collectedInteractables.Contains(collectableID) || GameManager.Instance.worldIDs.Contains(collectableID);
        if (entityFound && checkID)
        {
            Destroy(gameObject);
        }
        EnableKeyIcon(false);
    }
    public void Initialize(float timeBeforePickup, bool shouldCheckID)
    {
        pickupTime = timeBeforePickup;
        checkID = shouldCheckID;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        if(pickupTime > 0)
        {
            pickupTime -= Time.deltaTime;
        }
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= data.maxDistanceToCollect;
        if (inRange && pickupTime <= 0 && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction)
        {
            EnableKeyIcon(true);
            if (Input.GetKeyDown(data.pickupKey) && CanCollect())
            {
                Player.Instance.InInteraction = true;
                Player.Instance.anim.SetTrigger("pickUp");
                StartCoroutine(PickUp());
            }
        }
        else
        {
            EnableKeyIcon(false);
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
        GameManager.Instance.worldIDs.Add(collectableID);
        EnableKeyIcon(false);
        spriteRend.enabled = false;
        if (data.displayMessage)
        {
            yield return StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(pickupMessage));
        }
        yield return null;
        Player.Instance.InInteraction = false;
        Destroy(gameObject);
    }

    private void EnableKeyIcon(bool value)
    {
        keyIcon.SetActive(value);
    }
    
}
