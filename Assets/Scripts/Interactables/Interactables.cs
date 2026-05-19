using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    [SerializeField] InteractableData data;
    private bool inRange;

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
    public abstract void OnInteract();
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
