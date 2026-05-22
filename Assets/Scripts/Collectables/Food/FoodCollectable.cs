using UnityEngine;

public class FoodCollectable : Collectable
{
    public override void Collect()
    {
        Player.Instance.IncrementGrappleCharges();
    }
}
