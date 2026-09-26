using UnityEngine;
using System.Collections.Generic;

public class DialogueGraphNode : ScriptableObject
{
    // public enum Speaker { NPC, Player }
    // public Speaker speaker;
    public int nodeId;
    public bool hasChoices;
    public int nextNodeId;
    public DialogueGraphNode nextNode;
    public Condition[] continueConditions;

    public List<DialogueLine> lineList;

    // public void fillNode(Dictionary<int,DialogueGraphNode> nodes)
    // {
    //     if(nodeId == -1)
    //     {

    //     }
    // }
}
