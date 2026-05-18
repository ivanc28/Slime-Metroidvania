using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    Player player;

    Vector2 direction;
    float speed;
    float lifeTime;

    LayerMask grappleMask;
    Rigidbody2D rb;
    LineRenderer hookLine;

    bool attached;
    

    public void Initialize(
        Player p,
        Vector2 dir,
        float hookSpeed,
        float maxTime,
        LayerMask mask)
    {
        player = p;
        direction = dir;
        speed = hookSpeed;
        lifeTime = maxTime;
        grappleMask = mask;
        rb = GetComponent<Rigidbody2D>();
        hookLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!player.HookEqualsOneThrown(this) && !player.HookEqualsOneAttached(this))
        {
            Destroy(gameObject);
        }
        if (attached) return;

        rb.linearVelocity = direction * speed;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            player.HookMissed();
            Destroy(gameObject);
        }
    }
    void LateUpdate()
    {
        if (player.GetIsLocked())
        {
            hookLine.enabled = false;
        }
        else
        {
            hookLine.enabled = true;
            hookLine.SetPosition(0, player.transform.position);
            hookLine.SetPosition(1, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attached) return;
        if (((1 << collision.gameObject.layer) & grappleMask) != 0)
        {
            attached = true;
            Vector2 hitPoint = transform.position;
            player.HookAttached(hitPoint, this);
            rb.linearVelocity = Vector2.zero;
        }
    }
}