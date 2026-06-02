using UnityEngine;

public class ScissorsInteractable : Interactables
{
    public ScissorsBush bush;
    public override void OnInteract()
    {
        int spriteIndex = Random.Range(0, bush.afterCut.Length);
        bool firstCut = !GetRoomOfInteractable().bushStates.ContainsKey(GetInteractableID());
        if (firstCut)
        {
            SpawnCollectable();
        }
        bush.SwapSprite(spriteIndex);
        bush.SaveSpriteIndex(spriteIndex);
        bush.SpawnParticle();
    }
}
