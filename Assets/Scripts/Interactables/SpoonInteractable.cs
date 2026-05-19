using UnityEngine;

public class SpoonInteractable : Interactables
{
    public int numPebbles;
    public override void OnInteract()
    {
        Debug.Log($"I spewed out {numPebbles} pebbels!");
    }
}
