using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    [Tooltip("Percentage of player's X velocity that's added to the hook's velocity")]
    [SerializeField] Vector2 playerVelFactor;
    [SerializeField] float lengthBufferIfStillTimeLeft;
    [SerializeField] float lifetime;
    Player player;

    Vector2 direction;
    float speed;
    float length;

    LayerMask grappleMask;
    Rigidbody2D rb;
    LineRenderer hookLine;

    bool attached;
    Rigidbody2D playerRB;

    public void Initialize(Player p, Vector2 dir, float hookSpeed, float maxLength, float minLifetime, LayerMask mask)
    {
        player = p;
        direction = dir;
        speed = hookSpeed;
        length = maxLength;
        lifetime = minLifetime;
        grappleMask = mask;
        rb = GetComponent<Rigidbody2D>();
        hookLine = GetComponent<LineRenderer>();

        playerRB = player.GetComponent<Rigidbody2D>();
        playerVelFactor.x = Mathf.Clamp01(playerVelFactor.x);
        playerVelFactor.y = Mathf.Clamp01(playerVelFactor.y);
    }

    void Update()
    {
        if (!player.HookEqualsOneThrown(this) && !player.HookEqualsOneAttached(this))
        {
            Destroy(gameObject);
        }
        if (!attached)
        {
            Vector2 moveVel = direction * speed;
            moveVel.x += playerRB.linearVelocityX * playerVelFactor.x;
            moveVel.y += playerRB.linearVelocityY * playerVelFactor.y;
            rb.linearVelocity = moveVel;

            lifetime -= Time.deltaTime;
            float distanceFromPlayer = Vector2.Distance(transform.position, player.hookFirePoint.position);

            if (distanceFromPlayer > length && lifetime <= 0)
            {
                player.HookMissed();
                Destroy(gameObject);
            }
        }     
    }
    void LateUpdate()
    {
        if (attached && player.GetIsLocked())
        {
            hookLine.enabled = false;
        }
        else
        {
            hookLine.enabled = true;
            hookLine.SetPosition(0, player.hookFirePoint.position);
            hookLine.SetPosition(1, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attached) return;
        // Check if the hit layer is within the grapple mask
        if (((1 << collision.gameObject.layer) & grappleMask) != 0)
        {
            if (collision == player.currentAttachedCollider && collision.gameObject.CompareTag("Zipline"))
            {
                return;
            }
            attached = true;
            rb.linearVelocity = Vector2.zero;
            Vector2 hitPoint = collision.ClosestPoint(transform.position);
            transform.position = hitPoint;
            player.HookAttached(hitPoint, this, collision);
        }
    }

}