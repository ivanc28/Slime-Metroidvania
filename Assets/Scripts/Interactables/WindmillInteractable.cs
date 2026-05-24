using UnityEngine;

public class WindmillInteractable : Interactables
{
    public float spinForce;
    public float maxRotationSpeed;
    [SerializeField] Rigidbody2D rb;

    public override void MakeUpdate()
    {
        base.MakeUpdate();
        if(rb.angularVelocity > maxRotationSpeed)
        {
            rb.angularVelocity = maxRotationSpeed;
        }
    }
    public override void OnInteract()
    {
        rb.AddTorque(spinForce);
        Debug.Log("SPIN");
    }
}
