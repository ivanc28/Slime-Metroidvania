using System.Collections;
using UnityEngine;

public class PlayerTalking : MonoBehaviour
{
    public static NPC[] NPCsInRoom;
    public static bool inDialogue;
    private NPC currNearestNPC;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }
        NPC nearestNPC = GetNearestNPCInRange();
        if(currNearestNPC != nearestNPC || !Player.Instance.GetIsGrounded() || Player.Instance.InInteraction)
        {
            if(currNearestNPC != null)
            {
                currNearestNPC.EnableKeyIcon(false);
            }
            currNearestNPC = nearestNPC;
        }
        if(nearestNPC != null && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction)
        {
            nearestNPC.EnableKeyIcon(true);
        }
        if (Input.GetKeyDown(KeyCode.W) && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction)
        {
            StartCoroutine(InteractWithNPC(nearestNPC));
        }
    }

    private NPC GetNearestNPCInRange()
    {
        NPC nearestNPC = null;
        float closestDistance = Mathf.Infinity;
        foreach (NPC npc in NPCsInRoom)
        {
            float dist = Vector2.Distance(npc.transform.position, transform.position);
            if (dist <= npc.data.interactDistance && dist < closestDistance)
            {
                closestDistance = dist;
                nearestNPC = npc;
            }
        }
        return nearestNPC;
    }
    private IEnumerator InteractWithNPC(NPC npc)
    {
        if (npc != null)
        {
            Player.Instance.InInteraction = true;
            bool shouldStayInPlace = npc.forcePlayerMoveLeft && npc.forcePlayerMoveRight;

            if (!shouldStayInPlace)
            {   
                Player.Instance.rb.sharedMaterial = Player.Instance.data.someFriction;
                float talkDistance = Player.Instance.data.talkDistance;
                float getDistanceSpeed = Player.Instance.data.getDistanceSpeed;

                bool walkRight = npc.transform.position.x < transform.position.x;
                if (npc.forcePlayerMoveRight)
                    walkRight = true;
                if (npc.forcePlayerMoveLeft)
                    walkRight = false;

                while (Vector2.Distance(npc.transform.position, transform.position) < talkDistance)
                {
                    if (walkRight)
                    {
                        Player.Instance.rb.linearVelocityX = getDistanceSpeed;
                        Player.Instance.GetRenderer().flipX = false;
                    }
                    else
                    {
                        Player.Instance.rb.linearVelocityX = -getDistanceSpeed;
                        Player.Instance.GetRenderer().flipX = true;
                    }
                    yield return null;
                }
                Player.Instance.GetRenderer().flipX ^= true; // flip the player sprite (same as flipX = !flipX)
                yield return new WaitForSeconds(0.25f);
            }

            npc.Interact();
        }
    }

    public static void SetNPCsInRoom()
    {
        NPCsInRoom = FindObjectsByType<NPC>(FindObjectsSortMode.None);
    }
}
