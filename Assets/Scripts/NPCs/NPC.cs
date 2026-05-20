using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData data;
    public GameObject keyIcon;
    public SpriteRenderer npcRenderer;
    
    private int dialogueSequenceNum;

    private Vector2 dialoagueBoxPos;
    private bool choosingToSkipDialogue;
    private bool isInteracting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyIcon.GetComponent<SpriteRenderer>().sprite = data.keySprite;
        EnableKeyIcon(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isInteracting)
        {
            choosingToSkipDialogue = true;
            EndDialogue();
        }
    }

    public void Interact()
    {
        isInteracting = true;
        Debug.Log($"My name is {data.npcName} and I am talking");
    }
    public void NextDialogueSequence()
    {
        dialogueSequenceNum++;
    }

    private void EndDialogue()
    {
        Player.Instance.InInteraction = false;
        isInteracting = false;
    }

    public void EnableKeyIcon(bool enabled)
    {
        keyIcon.SetActive(enabled);
    }

    private IEnumerator DialogueSequence(TextMeshProUGUI text, string message, float delayBetweenChars, float delayAfterPeriod, float delayAfterComma)
    {
        text.text = message;
        text.maxVisibleCharacters = 0;
        int maxChars = 0;
        int i = 0;
        while (i < message.Length)
        {
            char nextChar = message[i];
            if (nextChar == '<')
            {
                int j = i;
                while (nextChar != '>')
                {
                    j++;
                    nextChar = message[j];
                }
                i = j;
                maxChars--;
            }
            i++;
            maxChars++;
            text.maxVisibleCharacters = maxChars;
            if (choosingToSkipDialogue) // choosingToSkipDialogue set in update
            {
                text.maxVisibleCharacters = message.Length;
                break;
            }
            if (nextChar == '.' || nextChar == '?')
            {
                yield return new WaitForSeconds(delayAfterPeriod);
            }
            else if (nextChar == ',')
            {
                yield return new WaitForSeconds(delayAfterComma);
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenChars);
            }
        }
        text.maxVisibleCharacters = message.Length;
    }

    private Vector2 CalculateDialogueBoxPos(bool forPlayer)
    {
        if (forPlayer)
        {
            Vector2 playerPos = Player.Instance.transform.position;
            return playerPos + Vector2.up * (Player.Instance.GetRenderer().bounds.size.y * 0.5f + Player.Instance.data.distanceDialogueAboveHeadOffset);
        }
        else
        {
            Vector2 npcPos = transform.position;
            return npcPos + Vector2.up * (npcRenderer.bounds.size.y * 0.5f + data.distanceDialogueAboveHeadOffset);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, data.interactDistance);
    }
}
