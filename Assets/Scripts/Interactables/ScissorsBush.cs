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
    //public void Cut()
    //{
    //    int currentIndex = room.bushStates.ContainsKey(interactable.GetInteractableID()) ? room.bushStates[interactable.GetInteractableID()] : -1;
    //    int spriteIndex = Random.Range(0, afterCut.Length - 1);
    //    Debug.Log($"curr index: {currentIndex}, random index: {spriteIndex}");
    //    if (spriteIndex >= currentIndex && currentIndex != -1)
    //    {
    //        spriteIndex++;
    //    }
    //    Debug.Log($"new sprite index if it was greater than curr index: {spriteIndex}");
    //    SaveSpriteIndex(spriteIndex);
    //    SwapSprite(spriteIndex);
    //}
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
