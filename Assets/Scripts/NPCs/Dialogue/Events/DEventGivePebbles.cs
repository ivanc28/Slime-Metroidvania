using UnityEngine;

[CreateAssetMenu(fileName = "GivePebbles", menuName = "ScriptableData/Dialogue/Event/GivePebblesEvent")]
public class DEventGivePebbles : DialogueEvent
{
    public int numPebbles;
    public string purchaseID;
    public AudioClip purchaseClip;
    public override void Invoke()
    {
        Player.Instance.currencyData.OnPurchase(numPebbles);
        Player.Instance.inventory.CompletePurchase(purchaseID);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(purchaseClip, 0.5f);
        }
    }
}
