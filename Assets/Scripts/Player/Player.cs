using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerData data;
    // Movement
    private float moveInput;
    // Jumping
    private bool pressedJump;
    private bool liftedJump;
    private bool isJumping;
    private bool isGrounded;
    private float jumpBufferTimer;
    private float coyoteTimer;
    [Header("Components")]
    public Rigidbody2D rb;
    [SerializeField] Collider2D col;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Transform feetPos;
    [SerializeField] HookProjectile hookPrefab;

    HookProjectile currHookAttached;
    HookProjectile currHookBeingThrown;
    bool isAttaching;
    [SerializeField] bool isLockedOnGrapple;
    Vector2 hookPoint;

    // Tools
    public PlayerTools tools;


    // Singleton
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        jumpBufferTimer = data.jumpBuffer;
        coyoteTimer = data.coyoteTime;
        tools = new PlayerTools();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
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
        }
        else
        {
            coyoteTimer = data.coyoteTime;
        }
        
        if (Input.GetKeyUp(KeyCode.Space) && isJumping && !isLockedOnGrapple)
        {
            liftedJump = true;
        }

        if (!isGrounded)
        {
            rb.sharedMaterial = data.frictionless;
        }
        else
        {
            rb.sharedMaterial = null;
        }

        // Fire grappling hook
        if (Input.GetMouseButtonDown(0))
        {
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
            }
            if (shouldJumpAfterDetach)
            {
                Jump();
            }
        }

        //// TEST
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    rb.linearVelocityX = 50 * moveInput;
        //}
    }

    private void FixedUpdate()
    {
        #region Grappling
        if (isAttaching)
        {
            Vector2 dir = ((Vector2)hookPoint - rb.position).normalized;
            rb.linearVelocity = dir * data.attachSpeed;

            float dist = Vector2.Distance(rb.position, hookPoint);
            isLockedOnGrapple = false;
            if (dist < 0.8f)
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
        // Movement
        Run(1);
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
                Debug.Log("Stopped jump");
            }
            liftedJump = false;
        }
        #endregion

        // Adjust gravity
        if (rb.linearVelocityY > 0)
        {
            if (isJumping)
            {
                rb.gravityScale = data.risingGravity;
            }
        }
        else
        {
            rb.gravityScale = data.fallingGravity;
            isJumping = false;
        }

        // Terminal fall speed
        if (rb.linearVelocityY < data.terminalFallVel)
        {
            rb.linearVelocityY = data.terminalFallVel;
        }

    }

    // References
    public SpriteRenderer GetRenderer()
    {
        return rend;
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
    private void Jump()
    {
        isJumping = true;
        rb.gravityScale = data.risingGravity;
        rb.linearVelocityY = data.jumpSpeed;
    }
    private void StopJump()
    {
        if(rb.linearVelocityY > 0)
        {
            rb.gravityScale = data.stopJumpGravity;
            isJumping = false;
        }
    }

    private void FireHook()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        HookProjectile hook = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        hook.Initialize(this, dir, data.hookSpeed, data.baseGrappleLength, data.minGrappleLifetime, data.grappleObjects);
        currHookBeingThrown = hook;
    }
    public void HookAttached(Vector2 point, HookProjectile hook)
    {
        if (currHookAttached != null && currHookAttached != hook)
        {
            Destroy(currHookAttached.gameObject);
        }

        currHookAttached = hook;
        currHookBeingThrown = null;

        hookPoint = point;
        isAttaching = true;

        rb.gravityScale = 0;
    }
    public void DetachHook()
    {
        isAttaching = false;
        if (!isLockedOnGrapple)
        {
            rb.gravityScale = data.fallingGravity;
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feetPos.position, data.feetRadius);

    }

}
