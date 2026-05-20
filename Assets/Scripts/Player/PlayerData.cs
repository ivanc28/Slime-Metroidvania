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
    public float noInputMomentumPreservation;
    public float noInputMomentumFactor = 0.85f;
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
    public float baseGrappleLength;
    public float minGrappleLifetime;
    public float attachSpeed;
    public float hookSpeed;
    public LayerMask grappleObjects;
    [Header("Bubble Blowing")]
    public KeyCode interactKey;
    public float blowTime;
    public float bubbleMoveSpeed;
    public float floatAccelAmount;
    public float floatDecelAmount;
}
