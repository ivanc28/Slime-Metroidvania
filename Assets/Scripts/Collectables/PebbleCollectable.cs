using UnityEngine;

public class PebbleCollectable : Collectable
{
    public int numPebbles;

    public override void Collect()
    {
        Player.Instance.currencyData.IncreaseCurrency(numPebbles);
    }
}
