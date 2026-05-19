using UnityEngine;

public class Interactables : MonoBehaviour
{
    [SerializeField] InteractableData data;
    private bool inRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(data.interactKey))
            {
                if (Player.Instance.tools.GetCurrTool() == data.requiredTool)
                {
                    OnInteract();
                    Destroy(gameObject);
                }
            }
        }    
       
    }
    public virtual void OnInteract()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
