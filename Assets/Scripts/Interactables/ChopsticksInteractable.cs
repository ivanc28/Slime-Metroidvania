using UnityEngine;

public class ChopsticksInteractable : Interactables
{
    public AudioClip[] collectClips;
    public override void OnInteract()
    {
        Player.Instance.currencyData.IncreaseCurrency(numPebbles);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(collectClips, 0.75f, true);
        }
    }
}
