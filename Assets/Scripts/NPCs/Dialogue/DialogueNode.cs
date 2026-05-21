using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "ScriptableData/Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public enum Speaker { NPC, Player }
    public Speaker speaker;
    [Tooltip("A one liner")]
    [TextArea(2,4)]
    public string line;
    [Tooltip("An optional event that gets called when this line ends")]
    public DialogueEvent dialogueEvent;
    [Tooltip("0 choices means this is the last line. 1 choice means no choices, go to next line. 2+ choices means we give the player dialogue options")]
    public List<DialogueChoice> choices;
}
