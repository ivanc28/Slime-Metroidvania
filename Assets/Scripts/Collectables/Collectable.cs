using System.Collections;
using UnityEngine;

public abstract class Collectable : Inspectable
{
    public CollectableData data;
    public string collectableID;
    [Tooltip("The message should fit one line")]
    public string pickupMessage;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;
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
        OnFocusChanged(false);
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
    }
    public abstract void Collect();
    public override bool CanInspect(Player player)
    {
        return base.CanInspect(player) && pickupTime <= 0;
    }
    public override IEnumerator Inspect(Player player)
    {
        Player.Instance.InInteraction = true;
        Player.Instance.anim.SetTrigger("pickUp");
        yield return new WaitForSeconds(data.pickupTime);
        Collect();
        AddCollectableToDB(collectableID);
        OnFocusChanged(false);
        spriteRend.enabled = false;
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(data.pickupClips, 1, true);
        }
        if (data.displayMessage)
        {
            yield return StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(pickupMessage));
        }
        yield return null;
        player.InInteraction = false;
        Destroy(gameObject);
    }
    private void AddCollectableToDB(string id)
    {
        room.collectedCollectables.Add(id);
        GameManager.Instance.worldIDs.Add(id);
    }
    
}
