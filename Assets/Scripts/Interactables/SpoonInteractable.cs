using UnityEngine;

public class SpoonInteractable : Interactables
{
    public int numPebbles;
    public override void OnInteract()
    {
        SpawnPebbles(numPebbles);
    }
}
