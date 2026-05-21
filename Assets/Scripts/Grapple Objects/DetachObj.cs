using UnityEngine;

public class DetachObj : GrappleObj
{
    public override void EffectOnPlayerContactAfterHook()
    {
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.rb.gravityScale = Player.Instance.data.fallingGravity;
    }
}
