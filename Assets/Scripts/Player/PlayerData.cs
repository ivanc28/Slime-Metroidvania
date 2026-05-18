using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableData/Player")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float baseMoveSpeed;
    public float runAccelAmount;
    public float runDecelAmount;
    public float accelInAir;
    public float decelInAir;
    public float maxExcessSpeed;
    public float momentumPreservation;
    public float highSpeedMomentumPreservation;
    public PhysicsMaterial2D frictionless;
    [Header("Jumping")]
    public float jumpSpeed;
    public float feetRadius;
    public LayerMask groundObjects;
    public float risingGravity;
    public float fallingGravity;
    public float stopJumpGravity;
    public float terminalFallVel;
    [Tooltip("How many seconds of input jump buffer")]
    public float jumpBuffer;
    [Tooltip("How many seconds of coyote time")]
    public float coyoteTime;
    [Header("Grapple")]
    public float grappleAttachSpeed;
    public float grappleThrowSpeed;
}
