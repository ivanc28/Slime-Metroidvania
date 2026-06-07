using System.Collections;
using UnityEngine;

public class PlayerTalking : MonoBehaviour
{
    public static NPC[] NPCsInRoom;
    public static bool inDialogue;
    private NPC currNearestNPC;
    [Header("For Gizmos Only")]
    [SerializeField] Transform gizmosFeetPos;
    [SerializeField] float gizmosCheckGroundLength;
    [SerializeField] float gizmosTalkDistance;
    private bool startedMovingForNPC;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }
        NPC nearestNPC = GetNearestNPCInRange();
        if(currNearestNPC != nearestNPC || !Player.Instance.GetIsGrounded() || Player.Instance.InInteraction || Player.Instance.GetIsLocked())
        {
            if(currNearestNPC != null)
            {
                currNearestNPC.EnableKeyIcon(false);
            }
            currNearestNPC = nearestNPC;
        }
        if(nearestNPC != null && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction && !Player.Instance.GetIsLocked())
        {
            nearestNPC.EnableKeyIcon(true);
        }
        if (Input.GetKeyDown(KeyCode.W) && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction && !Player.Instance.GetIsLocked() && !Player.Instance.GetIsAttaching() && !Player.Instance.GetHookBeingThrown())
        {
            StartCoroutine(InteractWithNPC(nearestNPC));
            startedMovingForNPC = true;
        }
    }

    private NPC GetNearestNPCInRange()
    {
        NPC nearestNPC = null;
        float closestDistance = Mathf.Infinity;
        foreach (NPC npc in NPCsInRoom)
        {
            if(npc != null)
            {
                float dist = Vector2.Distance(npc.transform.position, transform.position);
                if (dist <= npc.data.interactDistance && dist < closestDistance)
                {
                    closestDistance = dist;
                    nearestNPC = npc;
                }
            }
        }
        return nearestNPC;
    }
    private IEnumerator InteractWithNPC(NPC npc)
    {
        if (npc != null)
        {
            Player.Instance.InInteraction = true;

            // When both are checked, just stay in place
            bool shouldStayInPlace = npc.forcePlayerMoveLeft && npc.forcePlayerMoveRight;

            if (!shouldStayInPlace)
            {
                // A flag that we will check later, can ignore for now
                bool rayCheckStayInPlace = false;

                Player.Instance.rb.sharedMaterial = Player.Instance.data.someFriction;
                float talkDistance = Player.Instance.data.talkDistance;
                float getDistanceSpeed = Player.Instance.data.getDistanceSpeed;

                // Intial check which side the player should move towards
                bool walkRight = npc.transform.position.x < transform.position.x;
                // We could have optionally forced the player to move a certain direction
                if(npc.forcePlayerMoveLeft || npc.forcePlayerMoveRight)
                {
                    if (npc.forcePlayerMoveRight)
                        walkRight = true;
                    if (npc.forcePlayerMoveLeft)
                        walkRight = false;
                }
                // We chose not to force the player a certain direction, so lets calculate where we should actually go
                else
                {
                    // Use rays to check both sides of the player and make sure there is ground to stand on where we are trying to move
                    Vector2 feetPos = Player.Instance.GetFeetPos();
                    Vector2 rightOrigin = new Vector2(npc.transform.position.x + talkDistance, feetPos.y);
                    Vector2 leftOrigin = new Vector2(npc.transform.position.x - talkDistance, feetPos.y);
                    LayerMask ground = Player.Instance.data.groundObjects;
                    RaycastHit2D rightGroundCheck = Physics2D.Raycast(rightOrigin, Vector2.down, Player.Instance.data.checkGroundLength, ground);
                    RaycastHit2D leftGroundCheck = Physics2D.Raycast(leftOrigin, Vector2.down, Player.Instance.data.checkGroundLength, ground);
                    RaycastHit2D rightWallCheck = Physics2D.Raycast(new Vector2(npc.transform.position.x, feetPos.y), Vector2.right, talkDistance, ground);
                    RaycastHit2D leftWallCheck = Physics2D.Raycast(new Vector2(npc.transform.position.x, feetPos.y), Vector2.left, talkDistance, ground);

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
                                Debug.Log("case 1");
                            }
                            else
                            {
                                // Can't go this way either, let's stay in place
                                rayCheckStayInPlace = true;
                                Debug.Log("case 2");
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
                                Debug.Log("case 3");
                            }
                            else
                            {
                                // Can't go this way either, let's stay in place
                                rayCheckStayInPlace = true;
                                Debug.Log("case 4");
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
                    while (Vector2.Distance(npc.transform.position, transform.position) < talkDistance)
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
                        if(walkTimer > Player.Instance.data.maxWalkTime)
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
                    yield return new WaitForSeconds(0.25f);
                }
            }
            npc.Interact();
            startedMovingForNPC = false;
        }
    }
    public bool NPCEqualsCurrNPC(NPC npc)
    {
        return currNearestNPC == npc;
    }
    public bool MovingForNPC()
    {
        return startedMovingForNPC;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawRay(gizmosFeetPos.position + Vector3.right * gizmosTalkDistance, Vector2.down * gizmosCheckGroundLength);
    }
    public static void SetNPCsInRoom()
    {
        NPCsInRoom = FindObjectsByType<NPC>(FindObjectsSortMode.None);
    }
}
