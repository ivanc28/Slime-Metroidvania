using NUnit.Framework;
using System.Collections;
using System.Security;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public static NPC CurrentInteractingNPC { get; private set; }
    public NPCData data;
    public GameObject keyIcon;
    public SpriteRenderer npcRenderer;
    public Animator anim;
    public bool logWarnings;
    [Tooltip("Forces the player to move to the right of NPC when talking")]
    public bool forcePlayerMoveRight;
    [Tooltip("Forces the player to move to the left of NPC when talking")]
    public bool forcePlayerMoveLeft;
    [Header("Animation Stuff")]
    public bool alwaysTryFacePlayer;
    [Tooltip("How close the player must be from this NPC for the NPC to look at them")]
    public float lookAtPlayerThreshold;
    public bool neverFlipDir;
    //public bool onlyFacePlayerWhenSpeak;
    
    private DialogueNode currNode;
    private int currDialogueSequence;

    private int maxDialogueSequences;

    private TextMeshProUGUI currText;
    private bool choosingToSkipLine;
    private bool isInteracting;
    private bool doneWithLine;
    private bool makingAChoice;
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
        anim.logWarnings = logWarnings;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isInteracting && !doneWithLine && !makingAChoice)
        {
            choosingToSkipLine = true;            
        }

        // Only called with linear choice paths
        if (doneWithLine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                doneWithLine = false;
                StartCoroutine(AdvanceDialogue());
            }
        }

        bool playerClose = Vector2.Distance(transform.position, Player.Instance.transform.position) <= lookAtPlayerThreshold;
        if (alwaysTryFacePlayer)
        {
            if(playerClose)
            {
                SetFacingRight(transform.position.x < Player.Instance.transform.position.x);
            }
        }
        anim.SetBool("playerClose", playerClose);
    }

    /// <summary>
    /// Called from PlayerTalking because this NPC was the closest
    /// </summary>
    public void Interact()
    {
        CurrentInteractingNPC = this;
        isInteracting = true;
        EnableKeyIcon(false);
        SetFacingRight(transform.position.x < Player.Instance.transform.position.x);
        Condition skipCondition = data.dialogueSequences[currDialogueSequence].skipSequenceCondition;
        if (skipCondition != null && skipCondition.ConditionMet())
        {
            NextDialogueSequence();
        }
        currNode = data.dialogueSequences[currDialogueSequence].rootNode;
        SetDialogue(currNode);
    }
    private void SetDialogue(DialogueNode node)
    {
        AudioClip clip = null;
        if (node.speaker == DialogueNode.Speaker.NPC)
        {
            SpawnDialogueBubble(CalculateDialogueBoxPos(false), node.line, false);
            clip = data.talkingSFX;
        }
        else
        {
            SpawnDialogueBubble(CalculateDialogueBoxPos(true), node.line, true);
            clip = Player.Instance.data.talkSFX;
        }
        StartCoroutine(StartDialogueLine(currText, currText.text, clip, data.delayBetweenChars, data.delayAfterPeriod, data.delayAfterComma));
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
        CurrentInteractingNPC = null;
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
            currText.color = Player.Instance.data.textColor;
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
                finalText += $"{ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceNotHoverColor)}\n";
            }
            else
            {
                finalText += $"{ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceNotHoverColor)}";
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

    private IEnumerator StartDialogueLine(TextMeshProUGUI text, string message, AudioClip sfxClip, float delayBetweenChars, float delayAfterPeriod, float delayAfterComma)
    {
        text.text = message;
        text.maxVisibleCharacters = 0;
        int maxChars = 0;
        int i = 0;
        int sfxCharCount = 0;
        bool canPlaySFX = SoundManager.Instance != null && sfxClip != null;

        while (i < message.Length)
        {
            char nextChar = message[i];
            char nextNextChar = i < message.Length - 1 ? message[i + 1] : ' ';
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

            // play sfx
            bool isPunctuation = nextChar == '.' || nextChar == ',' || nextChar == '?' ||
                      nextChar == '!' || nextChar == '-' || nextChar == ' ';

            if (!isPunctuation)
            {
                sfxCharCount++;
                if (canPlaySFX && sfxCharCount % data.playSFXEveryXChars == 1)
                {
                    SoundManager.Instance.PlaySound(sfxClip, 1, true);
                }
            }

            if (choosingToSkipLine) // choosingToSkipDialogue set in update
            {
                text.maxVisibleCharacters = message.Length;
                break;
            }
            if (nextChar == '.' || (nextChar == '?' && nextNextChar == ' ') || (nextChar == '!' && nextNextChar == ' ') || (nextChar == '-' && nextNextChar == ' '))
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
        makingAChoice = true;
        yield return null;
        bool selectedChoice = false;
        int currChoice = choices.Length - 1;
        while (!IsChoiceAvailable(choices[currChoice]) && currChoice > 0)
        {
            currChoice--;
        }
        while (!selectedChoice)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectedChoice = true;
                break;
            }
            while (!IsChoiceAvailable(choices[currChoice]))
            {
                currChoice++;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                int newChoice = currChoice - 1;

                if (newChoice >= 0 && IsChoiceAvailable(choices[newChoice]))
                {
                    currChoice = newChoice;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                int newChoice = currChoice + 1;
                if (newChoice < choices.Length && IsChoiceAvailable(choices[newChoice]))
                {
                    currChoice = newChoice;
                }
            }
            string updateDialogueText = "";
            for(int i = 0; i < choices.Length; i++)
            {
                if (!IsChoiceAvailable(choices[i]))
                {
                    updateDialogueText += ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceDisabledColor);
                }
                else if (i == currChoice)
                {
                    updateDialogueText += ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceHoverColor);
                }
                else
                {
                    updateDialogueText += ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceNotHoverColor);
                }
                if(i < choices.Length - 1)
                {
                    updateDialogueText += "\n";
                }
            }
            currText.text = updateDialogueText;
            yield return null;
        }
        if (currNode.choices[currChoice].dialogueEvents.Length > 0)
        {
            Destroy(currDialogueBox);
            currDialogueBox = null;
            yield return StartCoroutine(CallEvents(currNode.choices[currChoice].dialogueEvents));
            yield return null;
            choosingToSkipLine = false;
        }
        choiceChosen = currChoice;
        NextDialogue();
        makingAChoice = false;
    }

    private bool IsChoiceAvailable(DialogueChoice choice)
    {
        return choice.condition == null || choice.condition.ConditionMet();
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

    private IEnumerator CallEvents(DialogueEvent[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i] != null)
            {
                yield return StartCoroutine(events[i].InvokeAndWait(this));
            }
        }
    }

    private IEnumerator AdvanceDialogue()
    {
        if (currNode.choices.Length > 0 && currNode.choices[0].dialogueEvents.Length > 0)
        {
            Destroy(currDialogueBox);
            currDialogueBox = null;
            yield return StartCoroutine(CallEvents(currNode.choices[0].dialogueEvents));
            yield return null;
            choosingToSkipLine = false;
        }
        choiceChosen = 0;
        NextDialogue();
    }
    public bool IsInteracting()
    {
        return isInteracting;
    }

    // Animations
    public void SetFacingRight(bool facingRight)
    {
        if (neverFlipDir)
            return;
        npcRenderer.flipX = !facingRight;
    }
    public void FaceRight()
    {
        if (neverFlipDir)
            return;
        npcRenderer.flipX = false;
    }
    public void FaceLeft()
    {
        if (neverFlipDir)
            return;
        npcRenderer.flipX = true;
    }
    public void SetMoving()
    {
        anim.SetBool("isMoving", true);
    }
    public void SetIdle()
    {
        anim.SetBool("isMoving", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, data.interactDistance);
    }
}
