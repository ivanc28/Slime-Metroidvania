using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData data;
    public GameObject keyIcon;
    public SpriteRenderer npcRenderer;
    
    private DialogueNode currNode;
    private int currDialogueSequence;

    private int maxDialogueSequences;

    private TextMeshProUGUI currText;
    private bool choosingToSkipLine;
    private bool isInteracting;
    private bool doneWithLine;
    private GameObject currDialogueBox;

    RoomData room;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (!room.npcStates.ContainsKey(data.npcID))
        {
            room.npcStates[data.npcID] = 0;
        }
        else
        {
            currDialogueSequence = room.npcStates[data.npcID];
        }
        keyIcon.GetComponent<SpriteRenderer>().sprite = data.keySprite;
        EnableKeyIcon(false);
        maxDialogueSequences = data.dialogueSequences.Length;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isInteracting && !doneWithLine)
        {
            choosingToSkipLine = true;            
        }

        if (doneWithLine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextDialogue();
                doneWithLine = false;
            }
        }
    }

    /// <summary>
    /// Called from PlayerTalking because this NPC was the closest
    /// </summary>
    public void Interact()
    {
        isInteracting = true;
        EnableKeyIcon(false);
        currNode = data.dialogueSequences[currDialogueSequence].rootNode;
        SetDialogue(currNode);
    }
    private void SetDialogue(DialogueNode node)
    {
        if (node.speaker == DialogueNode.Speaker.NPC)
        {
            SpawnDialogueBubble(CalculateDialogueBoxPos(false), node.line);
        }
        else
        {
            SpawnDialogueBubble(CalculateDialogueBoxPos(true), node.line);
        }
        StartCoroutine(StartDialogueLine(currText, currText.text, data.delayBetweenChars, data.delayAfterPeriod, data.delayAfterComma));
    }
    private void NextDialogue()
    {
        if(currNode.dialogueEvent != null)
        {
            currNode.dialogueEvent.Invoke();
        }
        if(currNode.choices.Count == 0)
        {
            // End dialogue
            EndDialogueSequence();
            Condition condition = data.dialogueSequences[currDialogueSequence].nextSequenceCondition;
            if (currDialogueSequence < maxDialogueSequences - 1 && (condition == null || condition.ConditionMet()))
            {
                NextDialogueSequence();
            }
        }
        else if (currNode.choices.Count == 1)
        {
            // Seek next dialogue
            Destroy(currDialogueBox);
            currDialogueBox = null;
            currNode = currNode.choices[0].nextNode;
            SetDialogue(currNode);
        }
        else
        {
            // Wait for player's choice

        }

    }
    private void NextDialogueSequence()
    {
        currDialogueSequence++;
        room.npcStates[data.npcID] = currDialogueSequence;
    }

    private void EndDialogueSequence()
    {       
        Destroy(currDialogueBox);
        currDialogueBox = null;
        StartCoroutine(ReleasePlayerNextFrame());
    }

    public void EnableKeyIcon(bool enabled)
    {
        keyIcon.SetActive(enabled);
    }

    private void SpawnDialogueBubble(Vector3 pos, string text)
    {
        DialogueBox box = Instantiate(data.dialogueBoxPrefab, pos, Quaternion.identity);
        box.Initalize(text);
        currText = box.dialogueText;
        currDialogueBox = box.gameObject;
    }

    private IEnumerator StartDialogueLine(TextMeshProUGUI text, string message, float delayBetweenChars, float delayAfterPeriod, float delayAfterComma)
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
            if (choosingToSkipLine) // choosingToSkipDialogue set in update
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
        text.maxVisibleCharacters = int.MaxValue;
        doneWithLine = true;
        choosingToSkipLine = false;
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
    private IEnumerator ReleasePlayerNextFrame()
    {
        yield return null;
        Player.Instance.InInteraction = false;
        isInteracting = false;
        Player.Instance.rb.sharedMaterial = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, data.interactDistance);
    }
}
