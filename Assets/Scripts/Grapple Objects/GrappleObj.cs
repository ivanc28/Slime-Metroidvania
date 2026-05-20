using UnityEngine;

public abstract class GrappleObj : MonoBehaviour
{
    // Components
    public SpriteRenderer objRenderer;
    public Collider2D objCollider;

    private bool hookAttached;
    private bool playerInsideCollider;
    private void Start()
    {
        MakeStart();
    }
    public virtual void MakeStart()
    {

    }
    private void Update()
    {
        MakeUpdate();
    }
    public virtual void MakeUpdate()
    {

    }
    private void FixedUpdate()
    {
        MakeFixedUpdate();
    }
    public virtual void MakeFixedUpdate()
    {
        if (playerInsideCollider && hookAttached)
        {
            EffectOnPlayerContactAfterHook();
        }
    }
    public virtual void EffectOnPlayerContact()
    {
        playerInsideCollider = true;
    }
    public abstract void EffectOnPlayerContactAfterHook();
    public virtual void EffectOnHookContact()
    {
        hookAttached = true;
    }
    public virtual void DetachHook()
    {
        hookAttached = false;
    }
    public bool IsHookAttached()
    {
        return hookAttached;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EffectOnPlayerContact();
            if (hookAttached)
            {
                EffectOnPlayerContactAfterHook();
            }
        }
        if (collision.gameObject.CompareTag("Hook"))
        {
            EffectOnHookContact();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInsideCollider = false;
        }
        if (collision.gameObject.CompareTag("Hook"))
        {
            DetachHook();
        }
    }
}
