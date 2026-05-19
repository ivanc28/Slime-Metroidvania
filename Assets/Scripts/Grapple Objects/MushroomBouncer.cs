using UnityEngine;

public class MushroomBouncer : GrappleObj
{
    public float bounceSpeed;

    public override void EffectOnPlayerContactAfterHook()
    {
        float playerXVel = Player.Instance.rb.linearVelocityX;
        DetachHook();
        Player.Instance.rb.gravityScale = Player.Instance.data.fallingGravity;
        Player.Instance.rb.linearVelocity = new Vector2(playerXVel, bounceSpeed);
    }
}
