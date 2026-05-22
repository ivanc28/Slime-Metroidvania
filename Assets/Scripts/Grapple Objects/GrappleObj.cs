using UnityEngine;

public abstract class GrappleObj : MonoBehaviour
{
    // Components
    public SpriteRenderer objRenderer;
    public Collider2D objCollider;

    private bool hookAttached;
    private Collider2D hookedCollider;
    public bool PlayerInsideCollider { get; private set; }
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
        if ((PlayerInsideCollider || Player.Instance.GetIsLocked()) && hookAttached)
        {
            EffectOnPlayerContactAfterHook();
        }
    }
    public virtual void EffectOnPlayerContact()
    {
        PlayerInsideCollider = true;
    }
    public abstract void EffectOnPlayerContactAfterHook();
    public virtual void EffectOnHookContact(Collider2D hitCollider)
    {
        hookedCollider = hitCollider;
        hookAttached = true;
        Debug.Log("YAY");
    }
    public virtual void DetachHook()
    {
        hookAttached = false;
        hookedCollider = null;
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
            EffectOnHookContact(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInsideCollider = false;
        }
        if (collision.gameObject.CompareTag("Hook"))
        {
            if (collision == hookedCollider) return;
            DetachHook();
        }
    }
}
