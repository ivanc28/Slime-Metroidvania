using System.Collections;
using UnityEngine;

public class PlayerTalking : MonoBehaviour
{
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
        //if (GameManager.Instance.GamePaused)
        //{
        //    return;
        //}
        //NPC nearestNPC = GetNearestNPCInRange();
        //if(currNearestNPC != nearestNPC || !Player.Instance.GetIsGrounded() || Player.Instance.InInteraction || Player.Instance.GetIsLocked())
        //{
        //    if(currNearestNPC != null)
        //    {
        //        currNearestNPC.EnableKeyIcon(false);
        //    }
        //    currNearestNPC = nearestNPC;
        //}
        //if(nearestNPC != null && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction && !Player.Instance.GetIsLocked())
        //{
        //    nearestNPC.EnableKeyIcon(true);
        //}
        //if (Input.GetKeyDown(KeyCode.Q) && Player.Instance.GetIsGrounded() && !Player.Instance.InInteraction && !Player.Instance.GetIsLocked() && !Player.Instance.GetIsAttaching() && !Player.Instance.GetHookBeingThrown())
        //{
        //    StartCoroutine(InteractWithNPC(nearestNPC));
        //    startedMovingForNPC = true;
        //}
    }

    //private NPC GetNearestNPCInRange()
    //{
    //    NPC nearestNPC = null;
    //    float closestDistance = Mathf.Infinity;
    //    if(NPCsInRoom != null)
    //    {
    //        foreach (NPC npc in NPCsInRoom)
    //        {
    //            if (npc != null)
    //            {
    //                float dist = Vector2.Distance(npc.transform.position, transform.position);
    //                if (dist <= npc.data.interactDistance && dist < closestDistance)
    //                {
    //                    closestDistance = dist;
    //                    nearestNPC = npc;
    //                }
    //            }
    //        }
    //    }
    //    return nearestNPC;
    //}
    public bool NPCEqualsCurrNPC(NPC npc)
    {
        return currNearestNPC == npc;
    }
    public bool MovingForNPC()
    {
        return startedMovingForNPC;
    }
    public void SetStartedMovingForNPC(bool value)
    {
        startedMovingForNPC = value;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawRay(gizmosFeetPos.position + Vector3.right * gizmosTalkDistance, Vector2.down * gizmosCheckGroundLength);
    }
}
