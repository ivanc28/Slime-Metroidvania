using NUnit.Framework;
using System.Collections;
using System.Security;
using TMPro;
using UnityEngine;

public class NPC : Inspectable
{
    public static NPC CurrentInteractingNPC { get; private set; }
    public NPCData data;
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
    private IEnumerator MovePlayerForNPC()
    {
        Player.Instance.InInteraction = true;
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.DetachHook();
        // When both are checked, just stay in place
        bool shouldStayInPlace = forcePlayerMoveLeft && forcePlayerMoveRight;

        if (!shouldStayInPlace)
        {
            // A flag that we will check later, can ignore for now
            bool rayCheckStayInPlace = false;

            Player.Instance.rb.sharedMaterial = Player.Instance.data.someFriction;
            float talkDistance = Player.Instance.data.talkDistance;
            float getDistanceSpeed = Player.Instance.data.getDistanceSpeed;

            // Intial check which side the player should move towards
            bool walkRight = transform.position.x < Player.Instance.transform.position.x;
            // We could have optionally forced the player to move a certain direction
            if (forcePlayerMoveLeft || forcePlayerMoveRight)
            {
                if (forcePlayerMoveRight)
                    walkRight = true;
                if (forcePlayerMoveLeft)
                    walkRight = false;
            }
            // We chose not to force the player a certain direction, so lets calculate where we should actually go
            else
            {
                // Use rays to check both sides of the player and make sure there is ground to stand on where we are trying to move
                Vector2 feetPos = Player.Instance.GetFeetPos();
                Vector2 rightOrigin = new Vector2(transform.position.x + talkDistance, feetPos.y);
                Vector2 leftOrigin = new Vector2(transform.position.x - talkDistance, feetPos.y);
                LayerMask ground = Player.Instance.data.groundObjects;
                RaycastHit2D rightGroundCheck = Physics2D.Raycast(rightOrigin, Vector2.down, Player.Instance.data.checkGroundLength, ground);
                RaycastHit2D leftGroundCheck = Physics2D.Raycast(leftOrigin, Vector2.down, Player.Instance.data.checkGroundLength, ground);
                RaycastHit2D rightWallCheck = Physics2D.Raycast(new Vector2(transform.position.x, feetPos.y), Vector2.right, talkDistance, ground);
                RaycastHit2D leftWallCheck = Physics2D.Raycast(new Vector2(transform.position.x, feetPos.y), Vector2.left, talkDistance, ground);

                // Now determine if we should actually go where we planned to go from the initial walkRight check
                if (walkRight)
                {
                    if (!rightGroundCheck || rightWallCheck)
                    {
                        // There's no ground to the right of us or there's a wall! Let's check the other way
                        if (leftGroundCheck && !leftWallCheck)
                        {
                            // There's ground and no wall, let's go left instead
                            walkRight = false;
                            //Debug.Log("case 1");
                        }
                        else
                        {
                            // Can't go this way either, let's stay in place
                            rayCheckStayInPlace = true;
                            //Debug.Log("case 2");
                        }
                    }
                }
                else
                {
                    if (!leftGroundCheck || leftWallCheck)
                    {
                        // There's no ground to the left of us or there's a wall! Let's check the other way
                        if (rightGroundCheck && !rightWallCheck)
                        {
                            // There's ground and no wall, let's go right instead
                            walkRight = true;
                            //Debug.Log("case 3");
                        }
                        else
                        {
                            // Can't go this way either, let's stay in place
                            rayCheckStayInPlace = true;
                            //Debug.Log("case 4");
                        }
                    }
                }
            }

            yield return null;
            Player.Instance.DetachHook();   // in case player presses interact and grapple at same time

            // When rayCheckStayInPlace is false, that means we found solid ground so we can safely move to the side
            if (!rayCheckStayInPlace)
            {
                bool hasTurnedAround = false;
                float walkTimer = 0;
                while (Vector2.Distance(transform.position, Player.Instance.transform.position) < talkDistance)
                {
                    if (walkRight)
                    {
                        Player.Instance.rb.linearVelocityX = getDistanceSpeed;
                        Player.Instance.GetRenderer().flipX = false;
                        hasTurnedAround = true;
                    }
                    else
                    {
                        Player.Instance.rb.linearVelocityX = -getDistanceSpeed;
                        Player.Instance.GetRenderer().flipX = true;
                        hasTurnedAround = true;
                    }

                    // we've tried getting distance for long enough so let's stop
                    if (walkTimer > Player.Instance.data.maxWalkTime)
                    {
                        break;
                    }
                    walkTimer += Time.deltaTime;

                    yield return null;
                }
                if (hasTurnedAround)
                {
                    Player.Instance.GetRenderer().flipX ^= true; // flip the player sprite (same as flipX = !flipX)
                }
                Player.Instance.rb.linearVelocity = Vector2.zero;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
    /// <summary>
    /// Called from PlayerTalking because this NPC was the closest
    /// </summary>
    public override IEnumerator Inspect(Player player)
    {
        Player.Instance.GetComponent<PlayerTalking>().SetStartedMovingForNPC(true);
        isInteracting = true;
        yield return MovePlayerForNPC();
        Player.Instance.GetComponent<PlayerTalking>().SetStartedMovingForNPC(false);
        CurrentInteractingNPC = this;
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
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
    }

    private IEnumerator StartDialogueLine(TextMeshProUGUI text, string message, AudioClip sfxClip, float delayBetweenChars, float delayAfterPeriod, float delayAfterComma)
    {
        text.text = message;
        text.maxVisibleCharacters = 0;
        int maxChars = 0;
        int i = 0;
        int sfxCharCount = 0;
        bool canPlaySFX = SoundManager.Instance != null && sfxClip != null;
        float carryover = 0f;

        while (i < message.Length)
        {
            bool breakOutOfOuterLoop = false;
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


            float timeTillNextChar = 0;
            if (nextChar == '.' || (nextChar == '?' && nextNextChar == ' ') || (nextChar == '!' && nextNextChar == ' ') || (nextChar == '-' && nextNextChar == ' '))
            {
                timeTillNextChar = delayAfterPeriod;
            }
            else if (nextChar == ',')
            {
                timeTillNextChar = delayAfterComma;
            }
            else
            {
                timeTillNextChar = delayBetweenChars;
            }
            float elapsed = -carryover; // start negative so previous overshoot goes into this wait
            carryover = 0f;
            while (elapsed  < timeTillNextChar)
            {
                if (choosingToSkipLine) // choosingToSkipDialogue set in update
                {
                    breakOutOfOuterLoop = true;
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (!breakOutOfOuterLoop)
            {
                carryover = elapsed - timeTillNextChar;
            }
            if (breakOutOfOuterLoop)
            {
                text.maxVisibleCharacters = message.Length;
                break;
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
                    updateDialogueText += $"<s>{ChangeTextColor(choices[i].choiceText, Player.Instance.data.choiceDisabledColor)}</s>";
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
