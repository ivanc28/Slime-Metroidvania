using UnityEngine;

public class FoodCollectable : Collectable
{
    public enum Type { Food, Drink }
    public Type consumableType;
    public override void Collect()
    {
        if (consumableType == Type.Food)
        {
            Player.Instance.IncrementGrappleCharges();
        }
        else
        {
            Player.Instance.IncreaseGrappleLength();
        }
        Player.Instance.SetFoodHolder(data.itemSprite);
        Player.Instance.anim.SetTrigger("eat");
    }
}
