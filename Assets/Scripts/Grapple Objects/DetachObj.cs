using UnityEngine;

public class DetachObj : GrappleObj
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float spinForce;
    [SerializeField] float maxSpinSpeed;
    public override void EffectOnPlayerContactAfterHook()
    {
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.SetGravityToFalling();
        rb.AddTorque(spinForce);
    }
}
