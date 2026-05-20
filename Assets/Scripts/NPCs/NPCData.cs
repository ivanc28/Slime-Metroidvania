using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableData/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Dialogue[] dialogues;
    public float interactDistance;
    public Sprite keySprite;
    [Tooltip("How much above or below we offset the dialogue box")]
    public float distanceDialogueAboveHeadOffset = 1f;
}
