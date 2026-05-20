using UnityEngine;

public class PlayerTalking : MonoBehaviour
{
    public static NPC[] NPCsInRoom;
    private NPC currNearestNPC;

    // Update is called once per frame
    void Update()
    {
        NPC nearestNPC = GetNearestNPCInRange();
        if(currNearestNPC != nearestNPC)
        {
            if(currNearestNPC != null)
            {
                currNearestNPC.EnableKeyIcon(false);
            }
            currNearestNPC = nearestNPC;
        }
        if(nearestNPC != null)
        {
            nearestNPC.EnableKeyIcon(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
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
        }
    }

    public static void SetNPCsInRoom()
    {
        NPCsInRoom = FindObjectsByType<NPC>(FindObjectsSortMode.None);
    }
}
