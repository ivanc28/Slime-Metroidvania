using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableData/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Dialogue[] dialogues;
    public float interactDistance;
    public Sprite keySprite;
}
