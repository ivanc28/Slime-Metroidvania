using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData data;
    public GameObject keyIcon;
    private int dialogueSequenceNum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyIcon.GetComponent<SpriteRenderer>().sprite = data.keySprite;
        EnableKeyIcon(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log($"My name is {data.npcName} and I am talking");
    }
    public void NextDialogueSequence()
    {
        dialogueSequenceNum++;
    }

    public void EnableKeyIcon(bool enabled)
    {
        keyIcon.SetActive(enabled);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, data.interactDistance);
    }
}
