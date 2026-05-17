using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D col;
    [SerializeField] SpriteRenderer rend;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
