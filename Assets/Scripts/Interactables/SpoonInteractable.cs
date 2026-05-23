using UnityEngine;

public class SpoonInteractable : Interactables
{
    public override void OnInteract()
    {
        SpawnPebbles(numPebbles);
    }
}
