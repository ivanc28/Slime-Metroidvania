using UnityEngine;

public class MushroomBouncer : GrappleObj
{
    public float bounceSpeed;
    public float umbrellaBounceSpeed;

    public override void EffectOnPlayerContactAfterHook()
    {
        float playerXVel = Player.Instance.rb.linearVelocityX;
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.SetGravityToFalling();
        if (Player.Instance.IsUsingUmbrella())
        {
            Player.Instance.rb.linearVelocity = new Vector2(playerXVel, umbrellaBounceSpeed);
        }
        else
        {
            Player.Instance.rb.linearVelocity = new Vector2(playerXVel, bounceSpeed);
        }
    }
}
