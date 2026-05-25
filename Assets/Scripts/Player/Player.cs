using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerData data;
    [SerializeField] public UIManager uiManager;
    // Movement
    private float moveInput;
    private bool canRun = true;
    private bool facingRight;
    // Jumping
    private bool pressedJump;
    private bool liftedJump;
    private bool isJumping;
    private bool isGrounded;
    private float jumpBufferTimer;
    private float coyoteTimer;
    private bool canAdjustGravity = true;
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    [SerializeField] Collider2D col;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Transform feetPos;
    [SerializeField] HookProjectile hookPrefab;
    public Transform hookFirePoint;

    HookProjectile currHookAttached;
    HookProjectile currHookBeingThrown;
    bool isAttaching;
    [SerializeField] bool isLockedOnGrapple;
    Vector2 hookPoint;

    // Tools
    public PlayerTools tools;

    // Grappling
    [HideInInspector] public int grappleCharges;
    private int maxGrappleCharges;
    [HideInInspector] public float grappleRechargeTime;
    [HideInInspector] public float grappleRechargeTimer;

    [HideInInspector] public Collider2D currentAttachedCollider;
    private float grappleLength;

    // Zipline
    private bool zipping;
    [HideInInspector] public int zipDirection; // -1: left; 1: right; 0: not zipped
    [HideInInspector] public bool attachingToZip;
    private Vector2 leftZipPoint;
    private Vector2 rightZipPoint;
    private Vector2 firstZipPoint;
    private Vector2 secondZipPoint;
    private Vector2 zipVector;
    [SerializeField] List<EdgeCollider2D> zipColliders;
    private float ziplineSpeed;
    private float zipCoyoteTimer;

    [Header("ToolSelection")]
    [SerializeField] GameObject toolSelectorCanvas;
    [SerializeField] Transform selectWheelCenter;
    [SerializeField] ToolOption[] toolOptions;
    private bool toolSelectorEnabled;
    private bool usingTool;

    [Header("Blowing Bubbles")]
    [SerializeField] ParticleSystem bubbleParticle;
    private bool bubbledState;
    private bool inBubble;
    private bool onBubble;

    //[Header("Umbrella")]
    private bool umbrellaState;

    //[Header("Currency")]
    public PlayerCurrency currencyData;
    //[Header("Inventory")]
    public PlayerInventory inventory;

    [Header("Animations")]
    public float flipTimeTheshold;
    private float flipTimer;
    // Interactions
    public bool InInteraction { get; set; }

    // Singleton
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Debug.Log("I existed HAHA!");
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        jumpBufferTimer = data.jumpBuffer;
        coyoteTimer = data.coyoteTime;
        tools = new PlayerTools();
        currencyData = new PlayerCurrency();
        inventory = new PlayerInventory();
        maxGrappleCharges = data.initialMaxGrappleCharges;
        grappleCharges = maxGrappleCharges;
        grappleRechargeTime = data.grappleRechargeTime;
        grappleRechargeTimer = 0;
        grappleLength = data.baseGrappleLength;
        ziplineSpeed = data.ziplineSpeedValue;
        zipCoyoteTimer = data.coyoteTime;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnableToolSelectionCanvas(false);
        tools.ClaimTool(PlayerTools.Tool.None);
        bubbleParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        FacingRight(true);
        //TESTING
        tools.ClaimTool(PlayerTools.Tool.Spoon);
        tools.ClaimTool(PlayerTools.Tool.Whisk);
        tools.SwapTool(PlayerTools.Tool.Spoon);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }
        // animations here? trying
        anim.SetFloat("yVel", rb.linearVelocityY);
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocityX));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isAttaching", isAttaching);
        anim.SetBool("isGrappling", isAttaching || currHookBeingThrown != null);
        anim.SetBool("isGrappleLocked", isLockedOnGrapple);
        anim.SetBool("isZipping", zipping);
        if (flipTimer > 0 && !isGrounded)
        {
            flipTimer -= Time.deltaTime;
        }
        if (isGrounded || zipping || isAttaching || currHookBeingThrown != null || isLockedOnGrapple || inBubble)
        {
            flipTimer = flipTimeTheshold;
        }
        anim.SetBool("canFlip", flipTimer <= 0);


        if (InInteraction)
        {
            return;
        }

        float move = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                move = -1;
                FacingRight(false);
            }
            if (Keyboard.current.dKey.isPressed)
            {
                move = 1;
                FacingRight(true);
            }
            if ((Keyboard.current.aKey.isPressed && Keyboard.current.dKey.isPressed) || (!Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed))
                move = 0;
        }
        moveInput = move;
        //moveInput = Input.GetAxisRaw("Horizontal");
        //if (Input.GetKey(KeyCode.A))
        //{
        //    moveInput = -1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    moveInput = 1;
        //}
        //if((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)))
        //{
        //    moveInput = 0;
        //}
        isGrounded = Physics2D.OverlapCircle(feetPos.position, data.feetRadius, data.groundObjects);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressedJump = true;
            jumpBufferTimer = data.jumpBuffer;
        }
        if (pressedJump)
        {
            jumpBufferTimer -= Time.deltaTime;

            if (jumpBufferTimer <= 0)
            {
                pressedJump = false;
            }
        }
        if (!isGrounded)
        {
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
            rb.sharedMaterial = data.frictionless;
        }
        else
        {
            coyoteTimer = data.coyoteTime;
            rb.sharedMaterial = null;
            // checking if charges can be gained, hook is not being thrown, and hook is not attached (there is a frame where hook is attached but slime still grounded where recharge can happen otherwise)
            if (grappleCharges < maxGrappleCharges && currHookBeingThrown == null && isAttaching == false && !isLockedOnGrapple)
            {
                if (grappleRechargeTimer < grappleRechargeTime)
                {
                    grappleRechargeTimer += Time.deltaTime;
                    UIManager.Instance.UpdateGrappleCharges();
                }
                else
                {
                    grappleCharges++;
                    grappleRechargeTimer = 0;
                    UIManager.Instance.UpdateGrappleCharges();
                }
            }
        } 
        
        if (Input.GetKeyUp(KeyCode.Space) && isJumping && !isLockedOnGrapple)
        {
            liftedJump = true;
        }

        // Fire grappling hook if charges are available
        if (Input.GetMouseButtonDown(0) && grappleCharges > 0)
        {
            grappleCharges--;
            UIManager.Instance.UpdateGrappleCharges();
            FireHook();
        }
        // Detach grappling hook
        if(Input.GetKeyDown(KeyCode.Space))
        {
            bool shouldJumpAfterDetach = false;
            if (isLockedOnGrapple || isAttaching)
            {
                shouldJumpAfterDetach = isLockedOnGrapple;
                DetachHook();
                SetZipping(false);
                SetZipDirection(0);
                Debug.Log("unzip");
            }
            else if (IsZipping() || zipCoyoteTimer > 0)
            {
                shouldJumpAfterDetach = true;
                SetZipping(false);
                SetZipDirection(0);
                Debug.Log("unzip");
            }
            if (shouldJumpAfterDetach)
            {
                Jump();
            }
            currentAttachedCollider = null;
        }

        // Zip Coyote Time
        if (!zipping)
        {
            if(zipCoyoteTimer > 0)
            {
                zipCoyoteTimer -= Time.deltaTime;
            }
        }
        else
        {
            coyoteTimer = data.coyoteTime;
        }

        #region Tool Selection
        if (!InInteraction && !usingTool)
        {
            if (Input.GetMouseButton(1))
            {
                if (!toolSelectorEnabled)
                {
                    EnableToolSelectionCanvas(true);
                    toolSelectorEnabled = true;
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (toolSelectorEnabled)
                {
                    SelectTool();
                    EnableToolSelectionCanvas(false);
                    toolSelectorEnabled = false;
                }
            }
        }

        #endregion

        #region Using Tools
        if (Input.GetKeyDown(data.interactKey) && ((!InInteraction && !inBubble && isGrounded && !isAttaching && !isLockedOnGrapple && currHookBeingThrown == null) || onBubble))
        {
            StartCoroutine(UseTool());
            EnableToolSelectionCanvas(false);
        }
        #endregion
        //// TEST
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    rb.linearVelocityX = 50 * moveInput;
        //}
    }

    private void FixedUpdate()
    {
        if (InInteraction || GameManager.Instance.GamePaused)
        {
            return;
        }
        #region Grappling
        if (isAttaching)
        {
            Vector2 dir = ((Vector2)hookPoint - rb.position).normalized;
            rb.linearVelocity = dir * data.attachSpeed;

            float dist = Vector2.Distance(rb.position, hookPoint);
            isLockedOnGrapple = false;
            Debug.Log(dist);
            if (dist < 1.05f)
            {
                isLockedOnGrapple = true;
                isAttaching = false;
            }
            return;
        }
        if (isLockedOnGrapple)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        #endregion
        // Movement]
        if (canRun)
        {
            Run(1);
        }
        #region Zipline
        if (IsZipping())
        {
            if (GetMoveInput() < 0 && zipDirection == 1)
            {
                Debug.Log("a");
                SetZipDirection(-1);
            }
            if (GetMoveInput() > 0 && zipDirection == -1)
            {
                Debug.Log("d");
                SetZipDirection(1);
            }
            rb.linearVelocity = zipDirection * ziplineSpeed * zipVector;
            if (Vector2.Distance(leftZipPoint, new Vector2(transform.position.x,transform.position.y)) <= 0.5f && zipDirection == -1)
            {
                SetZipping(false);
                bool leftRight = firstZipPoint.x < secondZipPoint.x;
                int index = zipColliders.IndexOf(currentAttachedCollider as EdgeCollider2D);
                if ((index == 0 && leftZipPoint == firstZipPoint) || (index == zipColliders.Count - 1 && leftZipPoint == secondZipPoint))
                {
                    SetZipDirection(0);
                    // Jump();
                    currentAttachedCollider = null;
                    rb.gravityScale = data.fallingGravity;
                    isJumping = false;
                }
                else
                {
                    if (leftZipPoint == firstZipPoint)
                    {
                        firstZipPoint = zipColliders[index-1].points[0];
                        secondZipPoint = zipColliders[index-1].points[1];
                        currentAttachedCollider = zipColliders[index-1];
                    }
                    else
                    {
                        firstZipPoint = zipColliders[index+1].points[0];
                        secondZipPoint = zipColliders[index+1].points[1];
                        currentAttachedCollider = zipColliders[index+1];
                    }
                    bool newLeftRight = firstZipPoint.x < secondZipPoint.x;
                    if (firstZipPoint.x > secondZipPoint.x)
                    {
                        leftZipPoint = secondZipPoint;
                        rightZipPoint = firstZipPoint;
                    }
                    else
                    {
                        leftZipPoint = firstZipPoint;
                        rightZipPoint = secondZipPoint;
                    }
                    if (leftRight != newLeftRight)
                    {
                        SetZipDirection(1);
                    }
                    AttachToZipline(currentAttachedCollider);
                }
            }
            if (Vector2.Distance(rightZipPoint, new Vector2(transform.position.x,transform.position.y)) <= 0.5f && zipDirection == 1)
            {
                SetZipping(false);
                bool leftRight = firstZipPoint.x < secondZipPoint.x;
                int index = zipColliders.IndexOf(currentAttachedCollider as EdgeCollider2D);
                // Debug.Log($"current index: {index}; current count: {zipColliders.Count}");
                if ((index == 0 && rightZipPoint == firstZipPoint) || (index == zipColliders.Count - 1 && rightZipPoint == secondZipPoint))
                {
                    SetZipDirection(0);
                    // Jump();
                    currentAttachedCollider = null;
                    rb.gravityScale = data.fallingGravity;
                    isJumping = false;
                }
                else
                {
                    Debug.Log($"current index: {index}; current count: {zipColliders.Count}");
                    if (rightZipPoint == firstZipPoint)
                    {
                        firstZipPoint = zipColliders[index-1].points[0];
                        secondZipPoint = zipColliders[index-1].points[1];
                        currentAttachedCollider = zipColliders[index-1];
                    }
                    else
                    {
                        firstZipPoint = zipColliders[index+1].points[0];
                        secondZipPoint = zipColliders[index+1].points[1];
                        currentAttachedCollider = zipColliders[index+1];
                    }
                    bool newLeftRight = firstZipPoint.x < secondZipPoint.x;
                    if (firstZipPoint.x > secondZipPoint.x)
                    {
                        SetZipDirection(-1);
                        leftZipPoint = secondZipPoint;
                        rightZipPoint = firstZipPoint;
                        Debug.Log("swap direction");
                        Debug.Log(zipDirection);
                    }
                    else
                    {
                        leftZipPoint = firstZipPoint;
                        rightZipPoint = secondZipPoint;
                        Debug.Log("no swap");
                    }
                    if (leftRight != newLeftRight)
                    {
                        SetZipDirection(-1);
                    }
                    AttachToZipline(currentAttachedCollider);
                }
            }
        }
        #endregion
        // Jumping
        #region Jumping
        if (pressedJump && (isGrounded || coyoteTimer > 0))
        {
            Jump();
            pressedJump = false;
        }
        if (liftedJump)
        {
            if (isJumping)
            {
                StopJump();
            }
            liftedJump = false;
        }
        #endregion

        // Adjust gravity
        if (canAdjustGravity)
        {
            if (rb.linearVelocityY > 0)
            {
                if (isJumping)
                {
                    SetGravityToRising();
                }
            }
            else
            {
                SetGravityToFalling();
                isJumping = false;
            }
        }

        // Terminal fall speed
        if (!zipping)
        {
            if (!umbrellaState)
            {
                if (rb.linearVelocityY < data.terminalFallVel)
                {
                    rb.linearVelocityY = data.terminalFallVel;
                }
            }
            else
            {
                if (rb.linearVelocityY < -data.umbrellaDescendSpeed)
                {
                    rb.linearVelocityY = -data.umbrellaDescendSpeed;
                }
            }
        }        

    }

    // References
    public SpriteRenderer GetRenderer()
    {
        return rend;
    }
    private void FacingRight(bool value)
    {
        facingRight = value;
        rend.flipX = !value;
    }
    public bool GetFacingRight()
    {
        return facingRight;
    }
  
    private void Run(float lerpAmount)
    {
        // Calculate the direction we want to move in and our desired velocity

        float targetSpeed = moveInput * data.baseMoveSpeed;
        // We can reduce our control using Lerp() this smooths changes to our direction and speed
        targetSpeed = Mathf.Lerp(rb.linearVelocityX, targetSpeed, lerpAmount);

        // Preserve momentum when moving in the same direction
        float currentSpeed = rb.linearVelocityX;
        
        #region Calculate AccelRate
        float accelRate;

        // Gets an acceleration value based on if we are accelerating 
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (isGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDecelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDecelAmount * data.decelInAir;
        }
        #endregion

        #region Conserve Momentum
        bool isOverspeeding = Mathf.Abs(currentSpeed) > data.baseMoveSpeed;

        if (isOverspeeding)
        {
            if (moveInput == 0)
            {
                accelRate *= data.noInputMomentumPreservation; // medium preservation
            }
            else if (currentSpeed * moveInput > 0)
            {
                accelRate *= data.highSpeedMomentumPreservation; // strongest preservation
            }
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.linearVelocityX;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void Jump()
    {
        isJumping = true;
        rb.gravityScale = data.risingGravity;
        rb.sharedMaterial = data.frictionless;
        rb.linearVelocityY = data.jumpSpeed;
    }
    private void StopJump()
    {
        if(rb.linearVelocityY > 0)
        {
            if (!umbrellaState)
            {
                rb.gravityScale = data.stopJumpGravity;
            }
            else
            {
                rb.gravityScale = data.umbrellaStopJumpGravity;
            }
            isJumping = false;
        }
    }
    public bool GetIsGrounded()
    {
        return isGrounded;
    }
    public void SetGravityToFalling()
    {
        if (!umbrellaState)
        {
            rb.gravityScale = data.fallingGravity;
        }
        else
        {
            rb.gravityScale = data.umbrellaFallingGravity;
        }
    }
    public void SetGravityToRising()
    {
        if (!umbrellaState)
        {
            rb.gravityScale = data.risingGravity;
        }
        else
        {
            rb.gravityScale = data.umbrellaJumpGravity;
        }
    }


    // ---- GRAPPLE ----
    private void FireHook()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - hookFirePoint.position;
        dir.Normalize();
        if(dir.x > 0)
        {
            FacingRight(true);
        }
        else
        {
            FacingRight(false);
        }
        Vector2 spawnPos = (Vector2)hookFirePoint.position + dir * data.hookSpawnOffset;
        HookProjectile hook = Instantiate(hookPrefab, spawnPos, Quaternion.identity);
        hook.Initialize(this, dir, data.hookSpeed, grappleLength, data.minGrappleLifetime, data.grappleObjects);
        currHookBeingThrown = hook;
    }
    public void HookAttached(Vector2 point, HookProjectile hook, Collider2D currentCollider)
    {
        if (currHookAttached != null && currHookAttached != hook)
        {
            Destroy(currHookAttached.gameObject);
        }

        currHookAttached = hook;
        currHookBeingThrown = null;

        hookPoint = point;
        isAttaching = true;
        isJumping = false;

        currentAttachedCollider = currentCollider;

        rb.gravityScale = 0;
    }
    public void DetachHook()
    {
        isAttaching = false;
        if (!isLockedOnGrapple)
        {
            SetGravityToFalling();
        }

        if (currHookAttached != null)
        {
            Destroy(currHookAttached.gameObject);
        }

        currHookAttached = null;
        currHookBeingThrown = null;
        isLockedOnGrapple = false;
    }
    public void HookMissed()
    {
        currHookBeingThrown = null;
    }
    public bool GetIsLocked()
    {
        return isLockedOnGrapple;
    }
    public bool HookEqualsOneAttached(HookProjectile other)
    {
        return other == currHookAttached;
    }
    public bool HookEqualsOneThrown(HookProjectile other)
    {
        return other == currHookBeingThrown;
    }
    public bool GetIsAttaching()
    {
        return isAttaching;
    }
    public bool GetHookBeingThrown()
    {
        return currHookBeingThrown != null;
    }
    public void IncrementGrappleCharges()
    {
        maxGrappleCharges++;
    }
    public void IncreaseGrappleLength()
    {
        grappleLength += data.grappleLengthIncrements;
    }
    // -------------------


    // ---- TOOLS --------
    public int GetToolOptionsLength()
    {
        return toolOptions.Length;
    }
    public float GetWheelAngle()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 selectPos = selectWheelCenter.position;
        Vector2 dir = mousePos - selectPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 67.5f; // adjust angle so that the top option is 0-45 deg
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }
    public ToolOption GetHoveredTool()
    {
        float angle = GetWheelAngle();
        int toolChoice = (int)(angle / (360 / toolOptions.Length));
        return toolOptions[toolChoice];
    }
    private void SelectTool()
    {
        float angle = GetWheelAngle();
        int toolChoice = (int)(angle / (360 / toolOptions.Length));
        toolChoice = Mathf.Clamp(toolChoice, 0, toolOptions.Length - 1);
        tools.SetCurrToolOption(toolOptions[toolChoice]);
        tools.SwapTool(toolOptions[toolChoice].tool);
        UIManager.Instance.UpdateToolDispay(toolOptions[toolChoice]);
        foreach(ToolOption option in toolOptions)
        {
            option.EnableToolBG(false);
        }
    }
    /// <summary>
    /// Used for UI Wheel Selector
    /// </summary>
    /// <param name="tool"></param>
    public void UnlockTool(PlayerTools.Tool tool)
    {
        foreach(ToolOption option in toolOptions)
        {
            if(option.tool == tool)
            {
                option.RemoveLock();
                return;
            }
        }
    }
    private void EnableToolSelectionCanvas(bool enabled)
    {
        toolSelectorCanvas.SetActive(enabled);
    }

    public IEnumerator UseTool()
    {
        if (tools.GetCurrTool() == PlayerTools.Tool.None)
        {
            yield break;
        }
        yield return null;
        usingTool = true;
        rb.sharedMaterial = data.someFriction;  // ensure we don't slide around when we start interacting
        InInteraction = true;
        // play tool animation or something
        yield return new WaitForSeconds(data.toolUseTime);
        if(tools.GetCurrTool() == PlayerTools.Tool.BubbleBlower)
        {
            BlowBubbles();
        }
        else if(tools.GetCurrTool() == PlayerTools.Tool.Umbrella)
        {
            ToggleUmbrella();
        }
        InInteraction = false;
        rb.sharedMaterial = null;
        usingTool = false;
    }
    public void BlowBubbles()
    {
        bubbledState = !bubbledState;
        if (bubbledState)
        {
            bubbleParticle.Play();
        }
        else
        {            
            bubbleParticle.Stop();
        }
    }
    public bool GetBubbledState()
    {
        return bubbledState;
    }
    public void SetInBubble(bool value)
    {
        inBubble = value;
        canRun = !value;
        canAdjustGravity = !value;
    }
    public bool IsInBubble()
    {
        return inBubble;
    }
    public void SetOnBubble(bool value)
    {
        onBubble = value;
    }
    public bool IsOnbubble()
    {
        return onBubble;
    }

    public void ToggleUmbrella()
    {
        umbrellaState = !umbrellaState;
    }
    public bool IsUsingUmbrella()
    {
        return umbrellaState;
    }
    // ------------

    // ---- Zipping ----
    public void SetZipping(bool value)
    {
        zipping = value;
        canRun = !value;
        canAdjustGravity = !value;
    }
    public bool IsZipping()
    {
        return zipping;
    }
    public void SetZipDirection(float n)
    {
        if (n < 0)
        {
            zipDirection = -1;
            FacingRight(false);
        }
        else if (n > 0)
        {
            zipDirection = 1;
            FacingRight(true);
        }
        else
        {
            zipDirection = 0;
        }
    }
    public float GetMoveInput()
    {
        return moveInput;
    }
    public void AttachToZipline(Collider2D collision)
    {
        Vector2 closest = collision.ClosestPoint(transform.position);
        closest.y -= 0.3f;
        Debug.Log(closest);
        transform.position = closest;
        if (zipDirection == 0)
        {
            SetZipDirection(rb.linearVelocityX);
        }
        Debug.Log(zipDirection);
        rb.linearVelocity = Vector2.zero;
        SetZipping(true);
        rb.gravityScale = 0;
        Zip();
    }
    void Zip()
    {
        Debug.Log("zip1");
        zipColliders = currentAttachedCollider.gameObject.GetComponent<ZiplineObj>().GetLineColliders();
        int index = zipColliders.IndexOf(currentAttachedCollider as EdgeCollider2D);
        
        firstZipPoint = zipColliders[index].points[0];
        secondZipPoint = zipColliders[index].points[1];
        if (firstZipPoint.x > secondZipPoint.x)
        {
            leftZipPoint = secondZipPoint;
            rightZipPoint = firstZipPoint;
        }
        else
        {
            leftZipPoint = firstZipPoint;
            rightZipPoint = secondZipPoint;
        }
        Debug.Log(leftZipPoint.x);
        Debug.Log(rightZipPoint.x);

        zipVector = (rightZipPoint - leftZipPoint).normalized;
    }
    // --------------

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zipline") && attachingToZip && isLockedOnGrapple && collision == currentAttachedCollider)
        {
            DetachHook();
            
            // lowLineCollider.enabled = true;
            // Player.Instance.rb.gravityScale = 50;
            Debug.Log("unhook");
            AttachToZipline(collision);
            attachingToZip = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feetPos.position, data.feetRadius);
    }

}
