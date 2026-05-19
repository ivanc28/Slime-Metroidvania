using UnityEngine;

public abstract class GrappleObj : MonoBehaviour
{
    private bool hookAttached;
    private bool playerInsideCollider;
    private void FixedUpdate()
    {
        if(playerInsideCollider && hookAttached)
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
    public void DetachHook()
    {
        hookAttached = false;
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
