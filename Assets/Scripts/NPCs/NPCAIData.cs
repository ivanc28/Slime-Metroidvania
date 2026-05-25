using UnityEngine;

[CreateAssetMenu(fileName = "NPCAIData", menuName = "ScriptableData/NPCAIData")]
public class NPCAIData : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpSpeed;
    public float risingGravity;
    public float fallingGravity;
    [Header("Pathing AI")]
    public LayerMask groundObjects;
    [Tooltip("Min amount of time before the NPC stops moving (in seconds)")]
    public float minMoveTime;
    [Tooltip("Max amount of time before the NPC stops moving (in seconds)")]
    public float maxMoveTime;
    [Tooltip("Min amount of time the NPC pauses (in seconds)")]
    public float minPauseTime;
    [Tooltip("Max amount of time the NPC pasues (in seconds)")]
    public float maxPauseTime;
    [Tooltip("Distance from a wall/edge before turning around")]
    public float feetCheckLength;
    [Tooltip("How long the down check ray is to check for edges")]
    public float groundCheckLength;
    [Tooltip("Distance above feet to start from to detect high walls (prob 2+ tiles high)")]
    public float highWallHeight;

}
