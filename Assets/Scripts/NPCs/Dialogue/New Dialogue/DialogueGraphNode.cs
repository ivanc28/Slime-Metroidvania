using UnityEngine;
using System.Collections.Generic;

public class DialogueGraphNode : ScriptableObject
{
    // public enum Speaker { NPC, Player }
    // public Speaker speaker;
    public DialogueGraphNode[] nextNodes;
    public Condition[] continueConditions;
    [Tooltip("A one liner. This should be EMPTY if there is more than 1 choice")]
    [TextArea(2,4)]
    public List<DialogueLine> lineList;
}
