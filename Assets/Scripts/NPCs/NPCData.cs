using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableData/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcID;
    public string npcName;
    public float interactDistance;
    public Sprite keySprite;

    [Header("Dialogue")]
    public DialogueSequence[] dialogueSequences;
    public DialogueBox dialogueBoxPrefab;
    [Tooltip("How much above or below we offset the dialogue box")]
    public float distanceDialogueAboveHeadOffset = 0.75f;
    [Header("Dialogue Speed")]
    public float delayBetweenChars;
    public float delayAfterPeriod;
    public float delayAfterComma;
    [Header("Dialogue Color")]
    public Color npcTextColor;
    public Color playerTextColor;
    public Color choiceHoverColor;
    public Color choiceNotHoverColor;
}
