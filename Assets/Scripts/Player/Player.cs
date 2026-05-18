using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData data;
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
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D col;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Transform feetPos;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        jumpBufferTimer = data.jumpBuffer;
        coyoteTimer = data.coyoteTime;
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
        
        if (Input.GetKeyUp(KeyCode.Space))
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
        //// TEST
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    rb.linearVelocityX = 50 * moveInput;
        //}
    }

    private void FixedUpdate()
    {
        // Movement
        Run(1);
        // Jumping
        #region
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
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput * data.baseMoveSpeed;
        //We can reduce our control using Lerp() this smooths changes to our direction and speed
        targetSpeed = Mathf.Lerp(rb.linearVelocityX, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (isGrounded)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDecelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDecelAmount * data.decelInAir;
        #endregion

        //#region Add Bonus Jump Apex Acceleration
        ////Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        //if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        //{
        //    accelRate *= Data.jumpHangAccelerationMult;
        //    targetSpeed *= Data.jumpHangMaxSpeedMult;
        //}
        //#endregion

        //#region Conserve Momentum
        ////We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        //if (Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocityX) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
        //{
        //    //Prevent any deceleration from happening, or in other words conserve our current momentum
        //    //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
        //    accelRate = 0;
        //}
        //#endregion
        #region Conserve Momentum

        float currentSpeed = rb.linearVelocityX;

        bool isOverspeeding = Mathf.Abs(currentSpeed) > Mathf.Abs(targetSpeed);

        bool sameDirection = Mathf.Sign(currentSpeed) == Mathf.Sign(targetSpeed);

        bool hasInput = Mathf.Abs(targetSpeed) > 0.01f;

        if (isOverspeeding && sameDirection && hasInput)
        {
            // How far above max speed are we?
            float excessSpeed = Mathf.Abs(currentSpeed) - Mathf.Abs(targetSpeed);

            // 0 = barely overspeeding
            // 1 = massively overspeeding
            float excessRatio = Mathf.Clamp01(excessSpeed / data.maxExcessSpeed);

            // reduce deceleration smoothly
            accelRate *= Mathf.Lerp(data.momentumPreservation, data.highSpeedMomentumPreservation,excessRatio);
        }

        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.linearVelocityX;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feetPos.position, data.feetRadius);

    }
}
