using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableData/Player")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float baseMoveSpeed;
    public float jumpSpeed;
    [Header("Grapple")]
    public float grappleAttachSpeed;
    public float grappleThrowSpeed;
}
