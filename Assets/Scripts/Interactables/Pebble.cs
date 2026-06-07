using UnityEngine;

public class Pebble : MonoBehaviour
{
    private float pickupTimer;
    private bool touchingPlayer;
    [SerializeField] public Rigidbody2D rb;
    private bool ignoreCollision;
    public float maxFallSpeed = 25f;
    [SerializeField] SpriteRenderer pebbleRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] AudioClip[] collectClips;
    public void Initialize(float pickDelay)
    {
        pickupTimer = pickDelay;
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.Instance.GetComponent<Collider2D>());
        ignoreCollision = true;
        pebbleRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
    private void Update()
    {
        if (pickupTimer > 0)
        {
            pickupTimer -= Time.deltaTime;
        }
        else
        {
            if (ignoreCollision)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.Instance.GetComponent<Collider2D>(), false);
                ignoreCollision = false;
            }
            if (touchingPlayer)
            {
                CollectPebble();
            }
        }
        if(rb.linearVelocityY < -maxFallSpeed)
        {
            rb.linearVelocityY = -maxFallSpeed;
        }
    }
    private void CollectPebble()
    {
        Player.Instance.currencyData.IncreaseCurrency(1);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(collectClips, 1, true);
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touchingPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touchingPlayer = false;
        }
    }
}
