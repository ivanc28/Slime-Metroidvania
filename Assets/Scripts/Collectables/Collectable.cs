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
    private void Start()
    {
        MakeStart();
    }
    public virtual void MakeStart()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.collectedCollectables.Contains(collectableID))
        {
            Destroy(gameObject);
        }
        EnableKeyIcon(false);
    }
    public void Initialize(float timeBeforePickup)
    {
        gameObject.SetActive(true);
        pickupTime = timeBeforePickup;
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
        EnableKeyIcon(false);
        spriteRend.enabled = false;
        UIManager.Instance.DisplayCollectText(pickupMessage);
        while (!Input.GetKey(KeyCode.Space))
        {
            yield return null;
        }
        yield return null;
        Player.Instance.InInteraction = false;
        UIManager.Instance.DisableCollectText();
        Destroy(gameObject);
    }

    private void EnableKeyIcon(bool value)
    {
        keyIcon.SetActive(value);
    }
    
}
