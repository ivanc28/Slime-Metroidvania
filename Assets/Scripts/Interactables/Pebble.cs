using UnityEngine;

public class Pebble : MonoBehaviour
{
    private float pickupTimer;
    private bool touchingPlayer;
    [SerializeField] public Rigidbody2D rb;
    public void Initialize(float pickDelay)
    {
        pickupTimer = pickDelay;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(pickupTimer > 0)
        {
            pickupTimer -= Time.deltaTime;
        }
        else
        {
            if (touchingPlayer)
            {
                CollectPebble();
            }
        }
    }
    private void CollectPebble()
    {
        Player.Instance.currencyData.IncreaseCurrency(1);
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
