using UnityEngine;

public class ChopsticksInteractable : Interactables
{
    public override void OnInteract()
    {
        Player.Instance.currencyData.IncreaseCurrency(numPebbles);
    }
}
