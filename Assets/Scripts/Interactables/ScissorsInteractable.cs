using UnityEngine;

public class ScissorsInteractable : Interactables
{
    public ScissorsBush bush;
    public AudioClip[] leavesClips;
    public override void OnInteract()
    {
        bool firstCut = !GetRoomOfInteractable().bushStates.ContainsKey(GetInteractableID());

        int currentIndex = firstCut ? -1 : GetRoomOfInteractable().bushStates[GetInteractableID()];
        int spriteIndex = Random.Range(0, bush.afterCut.Length - 1);
        if (currentIndex != -1 && spriteIndex >= currentIndex)
        {
            spriteIndex++;
        }

        if (firstCut)
        {
            SpawnCollectable();
        }
        bush.SwapSprite(spriteIndex);
        bush.SaveSpriteIndex(spriteIndex);
        bush.SpawnParticle();
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(leavesClips, 0.3f, true);
        }
    }
}
