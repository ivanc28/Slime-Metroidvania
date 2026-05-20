using UnityEngine;

public class PlayerTalking : MonoBehaviour
{
    public static NPC[] NPCsInRoom;
    public static bool inDialogue;
    private NPC currNearestNPC;

    // Update is called once per frame
    void Update()
    {
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
            InteractWithNPC(nearestNPC);
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
    private void InteractWithNPC(NPC npc)
    {        
        if (npc != null)
        {
            npc.Interact();
            Player.Instance.InInteraction = true;
        }
    }

    public static void SetNPCsInRoom()
    {
        NPCsInRoom = FindObjectsByType<NPC>(FindObjectsSortMode.None);
    }
}
