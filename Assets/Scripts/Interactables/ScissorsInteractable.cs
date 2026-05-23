using UnityEngine;

public class ScissorsInteractable : Interactables
{
    public ScissorsBush bush;
    public override void OnInteract()
    {
        bush.SwapSprite(true);
        SpawnCollectable();
        bush.SpawnParticle();
    }
}
