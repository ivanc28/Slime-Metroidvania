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
    public PhysicsMaterial2D someFriction;
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
    public float grappleLengthIncrements;
    public float minGrappleLifetime;
    public float attachSpeed;
    public float hookSpeed;
    public float hookSpawnOffset;
    public LayerMask grappleObjects;
    public int initialMaxGrappleCharges;
    public float grappleRechargeTime;
    [Header("Tools")]
    public float toolUseTime;
    [Header("Bubble Blowing")]
    public KeyCode interactKey;
    public float blowTime;
    public float bubbleMoveSpeed;
    public float floatAccelAmount;
    public float floatDecelAmount;
    [Header("Umbrella")]
    public float umbrellaJumpGravity;
    public float umbrellaFallingGravity;
    public float umbrellaStopJumpGravity;
    public float umbrellaDescendSpeed;
    [Header("NPC Interaction")]
    public Color textColor;
    public Color choiceHoverColor;
    public Color choiceNotHoverColor;
    public Color choiceDisabledColor;
    public float talkDistance;
    public float getDistanceSpeed;
    [Tooltip("The max amount of time the player will try to get distance from the NPC")]
    public float maxWalkTime;
    [Tooltip("How far the down vector is to check if there is ground where we are trying to walk to get distance from the NPC")]
    public float checkGroundLength;
    [Header("Zipline")]
    public float ziplineSpeedValue;
    [Header("Inventory")]
    public ToolInventoryData[] toolUIData;
}
