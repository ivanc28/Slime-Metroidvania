using UnityEngine;

public class NPCAI : MonoBehaviour
{
    public NPCAIData data;
    [Tooltip("When on, the NPC will move back and forth")]
    public bool enableMovement;
    public bool enableJumping;
    public bool faceLeftOnStart;
    public SpriteRenderer npcRenderer;
    public Transform feetPos;
    private Rigidbody2D rb;
    private float moveTimer;
    private float pauseTimer;
    private float moveDir;
    private bool isJumping;
    private float jumpCooldown;
    private float jumpCooldownTime = 0.2f;
    private Animator anim;

    private NPC npc;
    private PlayerTalking playerTalking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npc = GetComponent<NPC>();
        rb = GetComponent<Rigidbody2D>();
        playerTalking = Player.Instance.GetComponent<PlayerTalking>();
        if (enableMovement)
        {
            // pick between moving left or right at first
            int roll = Random.Range(0, 2);
            if(roll == 0)
            {
                SetFacingRight(true);
            }
            else
            {
                SetFacingRight(false);
            }
            ResetMoveTimer();
        }
        else
        {
            SetFacingRight(!faceLeftOnStart);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //// Setting animator parameters
        //anim.SetFloat("moveDir", moveDir);
        //anim.SetBool("isMoving", Mathf.Abs(rb.linearVelocityX) > 0.01f);
        //anim.SetBool("isGrounded", IsGrounded());
        //anim.SetBool("isJumping", isJumping);
        //anim.SetBool("isInteracting", npc.IsInteracting());

        SetGravityScale();
        if (enableMovement && !npc.IsInteracting() && (!playerTalking.MovingForNPC() || !playerTalking.NPCEqualsCurrNPC(npc)))
        {
            if (moveTimer > 0)
            {
                // Move
                Move(moveDir);
                if(!GroundDetected() || (LowWallDetected() && HighWallDetected()))
                {
                    SwitchDirections();
                }
                else if(LowWallDetected() && !HighWallDetected())
                {
                    if (enableJumping && !isJumping)
                    {
                        Jump();
                    }
                    else
                    {
                        SwitchDirections();
                    }
                }
                moveTimer -= Time.deltaTime;
            }
            else if (pauseTimer > 0)
            {
                Move(0);
                pauseTimer -= Time.deltaTime;
            }
            else
            {
                ResetMoveTimer();
                ResetPauseTimer();
            }
        }
        else
        {
            Move(0);
        }

        if(jumpCooldown > 0)
{
            jumpCooldown -= Time.deltaTime;
        }

        if (IsGrounded() && rb.linearVelocityY <= 0 && jumpCooldown <= 0)
        {
            isJumping = false;
        }

    }

    private void Move(float dir)
    {
        rb.linearVelocityX = dir * data.moveSpeed;
    }
    private void Jump()
    {
        rb.linearVelocityY = data.jumpSpeed;
        isJumping = true;
        jumpCooldown = jumpCooldownTime;
    }
    private void SetGravityScale()
    {
        if(rb.linearVelocityY > 0)
        {
            rb.gravityScale = data.risingGravity;
        }
        else
        {
            rb.gravityScale = data.fallingGravity;
        }
    }
    private bool LowWallDetected()
    {
        Vector2 rayDir = moveDir == 1 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(feetPos.position, rayDir, data.feetCheckLength, data.groundObjects);
        return hit && !isJumping;
    }
    private bool HighWallDetected()
    {
        Vector2 startPos = feetPos.position;
        startPos.y += data.highWallHeight;
        Vector2 rayDir = moveDir == 1 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDir, data.feetCheckLength, data.groundObjects);
        return hit && !isJumping;
    }
    private bool GroundDetected()
    {
        Vector2 startPos = feetPos.position;
        startPos.x += moveDir == 1 ? data.feetCheckLength : -data.feetCheckLength;
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, data.groundCheckLength, data.groundObjects);
        return hit;
    }
    private void SwitchDirections()
    {
        moveDir *= -1;
        npcRenderer.flipX = !npcRenderer.flipX;
    }
    private void SetFacingRight(bool faceRight)
    {
        if (faceRight)
        {
            moveDir = 1;
        }
        else
        {
            moveDir = -1;
        }
        npcRenderer.flipX = !faceRight;
    }
    private void ResetMoveTimer()
    {
        moveTimer = Random.Range(data.minMoveTime, data.maxMoveTime);
    }
    private void ResetPauseTimer()
    {
        pauseTimer = Random.Range(data.minPauseTime, data.maxPauseTime);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(feetPos.position, Vector2.down, data.groundCheckLength, data.groundObjects);
        return hit;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = feetPos.position;
        Gizmos.color = Color.pink;
        Gizmos.DrawRay(origin, Vector2.right * data.feetCheckLength);
        Vector2 highOrigin = new Vector2(origin.x, origin.y + data.highWallHeight);
        Gizmos.DrawRay(highOrigin, Vector2.right * data.feetCheckLength);
        Vector2 downOrigin = new Vector2(origin.x + data.feetCheckLength, origin.y);
        Gizmos.DrawRay(downOrigin, Vector2.down * data.groundCheckLength);
    }
}
