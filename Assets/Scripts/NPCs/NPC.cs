using NUnit.Framework;
using System.Collections;
using System.Security;
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
    private int choiceChosen;
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

        // Only called with linear choice paths
        if (doneWithLine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currNode.choices.Length > 0 && currNode.choices[0].dialogueEvent != null)
                {
                    currNode.choices[0].dialogueEvent.Invoke();
                }
                choiceChosen = 0;
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
            SpawnDialogueBubble(CalculateDialogueBoxPos(false), node.line, false);
        }
        else
        {
            SpawnDialogueBubble(CalculateDialogueBoxPos(true), node.line, true);
        }
        StartCoroutine(StartDialogueLine(currText, currText.text, data.delayBetweenChars, data.delayAfterPeriod, data.delayAfterComma));
    }
    private void SetChoices(DialogueNode node)
    {
        if(node.speaker == DialogueNode.Speaker.NPC)
        {
            Debug.LogWarning("The NPC shouldn't have choices");
        }
        SpawnChoicesBubble(CalculateDialogueBoxPos(true), node.choices);
        StartCoroutine(PromptPlayerChoice(node.choices));
    }
    private void NextDialogue()
    {
        // Try to call the event function
        //if (currNode.choices.Length > 0)
        //{
        //    if (currNode.choices[choiceChosen].dialogueEvent != null)
        //    {
        //        currNode.choices[choiceChosen].dialogueEvent.Invoke();
        //    }
        //}
        // Either no more choices or the choice does not have a nextNode (we go into the block when nextNode = null too b/c might've had an event w/ no nextNode)
        if (currNode.choices.Length == 0 || currNode.choices[choiceChosen].nextNode == null)
        {
            // End dialogue
            EndDialogueSequence();
            Condition condition = data.dialogueSequences[currDialogueSequence].nextSequenceCondition;
            if (currDialogueSequence < maxDialogueSequences - 1 && (condition == null || condition.ConditionMet()))
            {
                NextDialogueSequence();
            }
        }
        else /*(currNode.choices.Length == 1)*/
        {
            // Seek next dialogue
            if (currNode.choices[choiceChosen].nextNode.choices.Length <= 1)
            {
                Destroy(currDialogueBox);
                currDialogueBox = null;
                currNode = currNode.choices[choiceChosen].nextNode;
                SetDialogue(currNode);
            }

            // Seek to a choice for the player
            else
            {
                Destroy(currDialogueBox);
                currDialogueBox = null;
                currNode = currNode.choices[choiceChosen].nextNode;
                SetChoices(currNode);
            }
        }
        //else
        //{
        //    // NEED FIX
        //    // Wait for player's choice
        //    Destroy(currDialogueBox);
        //    currDialogueBox = null;
        //    currNode = currNode.choices[0].nextNode;
        //    SetChoices(currNode);
        //}

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

    private void SpawnDialogueBubble(Vector3 pos, string text, bool forPlayer)
    {
        DialogueBox box = Instantiate(data.dialogueBoxPrefab, pos, Quaternion.identity);
        box.Initalize(text);
        currText = box.dialogueText;
        currDialogueBox = box.gameObject;
        if (forPlayer)
        {
            currText.color = data.playerTextColor;
        }
        else
        {
            currText.color = data.npcTextColor;
        }
    }
    private void SpawnChoicesBubble(Vector3 pos, DialogueChoice[] choices)
    {
        DialogueBox box = Instantiate(data.dialogueBoxPrefab, pos, Quaternion.identity);
        string finalText = "";
        for (int i = 0; i < choices.Length; i++)
        {
            if(i < choices.Length - 1)
            {
                finalText += $"{ChangeTextColor(choices[i].choiceText, data.choiceNotHoverColor)}\n";
            }
            else
            {
                finalText += $"{ChangeTextColor(choices[i].choiceText, data.choiceNotHoverColor)}";
            }
        }
        box.Initalize(finalText);
        currText = box.dialogueText;
        currDialogueBox = box.gameObject;
    }

    private string ChangeTextColor(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
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
    private IEnumerator PromptPlayerChoice(DialogueChoice[] choices)
    {
        yield return null;
        bool selectedChoice = false;
        int currChoice = 0;
        while (!selectedChoice)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectedChoice = true;
                break;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                currChoice--;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currChoice++;
            }
            currChoice = Mathf.Clamp(currChoice, 0, choices.Length - 1);
            string updateDialogeText = "";
            for(int i = 0; i < choices.Length; i++)
            {
                if(i == currChoice)
                {
                    updateDialogeText += ChangeTextColor(choices[i].choiceText, data.choiceHoverColor);
                }
                else
                {
                    updateDialogeText += ChangeTextColor(choices[i].choiceText, data.choiceNotHoverColor);
                }
                if(i < choices.Length - 1)
                {
                    updateDialogeText += "\n";
                }
            }
            currText.text = updateDialogeText;
            yield return null;
        }
        if (currNode.choices[currChoice].dialogueEvent != null)
        {
            currNode.choices[currChoice].dialogueEvent.Invoke();
        }
        choiceChosen = currChoice;
        NextDialogue();
    }
    private Vector2 CalculateDialogueBoxPos(bool forPlayer)
    {
        if (forPlayer)
        {
            Vector2 playerPos = Player.Instance.transform.position;
            return playerPos + Vector2.up * (Player.Instance.GetRenderer().bounds.size.y * 0.5f + data.distanceDialogueAboveHeadOffset);
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
