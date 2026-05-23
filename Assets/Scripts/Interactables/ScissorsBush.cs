using UnityEngine;

public class ScissorsBush : MonoBehaviour
{
    [SerializeField] ScissorsInteractable interactable;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Sprite beforeCut;
    [SerializeField] Sprite afterCut;
    public GameObject cutParticle;
    private RoomData room;
    private void Start()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.collectedInteractables.Contains(interactable.GetInteractableID()))
        {
            SwapSprite(true);
        }
    }
    public void SwapSprite(bool isCut)
    {
        if (isCut)
        {
            rend.sprite = afterCut;
        }
        else
        {
            rend.sprite = beforeCut;
        }
    }
    public void SpawnParticle()
    {
        if(cutParticle != null)
        {
            Instantiate(cutParticle, transform.position, Quaternion.identity);
        }
    }
}
