using UnityEngine;

public class MushroomBouncer : GrappleObj
{
    public float bounceSpeed;
    public float umbrellaBounceSpeed;
    public AudioClip bounceClip;
    public override void EffectOnPlayerContactAfterHook()
    {
        float playerXVel = Player.Instance.rb.linearVelocityX;
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.SetGravityToFalling();
        anim.SetTrigger("bounce");
        if (Player.Instance.IsUsingUmbrella())
        {
            Player.Instance.rb.linearVelocity = new Vector2(playerXVel, umbrellaBounceSpeed);
        }
        else
        {
            Player.Instance.rb.linearVelocity = new Vector2(playerXVel, bounceSpeed);
        }
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(bounceClip, 1, true);
        }
    }
}
