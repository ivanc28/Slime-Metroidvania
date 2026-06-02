using UnityEngine;

public class ScissorsBush : MonoBehaviour
{
    [SerializeField] ScissorsInteractable interactable;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Sprite beforeCut;
    public Sprite[] afterCut;
    public GameObject cutParticle;
    private RoomData room;
    private void Start()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.bushStates.TryGetValue(interactable.GetInteractableID(), out int spriteIndex))
        {
            SwapSprite(spriteIndex);
        }
    }
    public void Cut()
    {
        int spriteIndex = Random.Range(0, afterCut.Length);
        SaveSpriteIndex(spriteIndex);
        SwapSprite(spriteIndex);
    }
    public void SwapSprite(int index)
    {
        rend.sprite = afterCut[index];
    }
    public void SaveSpriteIndex(int index)
    {
        room.bushStates[interactable.GetInteractableID()] = index;
    }
    public void SpawnParticle()
    {
        if(cutParticle != null)
        {
            Instantiate(cutParticle, transform.position, Quaternion.identity);
        }
    }
}
