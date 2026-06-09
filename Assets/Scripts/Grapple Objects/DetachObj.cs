using UnityEngine;

public class DetachObj : GrappleObj
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float spinForce;
    [SerializeField] float maxSpinSpeed;
    [SerializeField] AudioClip spinClip;
    public override void MakeUpdate()
    {
        base.MakeUpdate();
        if(rb.angularVelocity > maxSpinSpeed)
        {
            rb.angularVelocity = maxSpinSpeed;
        }
    }
    public override void EffectOnPlayerContactAfterHook()
    {
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.SetGravityToFalling();
        rb.AddTorque(spinForce);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(spinClip, 0.2f, true, 1f, 1.15f);
        }
    }
}
