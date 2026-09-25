using UnityEngine;
using System.Collections.Generic;
using System;

public class DialogueGraph : MonoBehaviour
{
    public List<DialogueGraphNode> nodes;
    public DialogueGraphNode rootNode;
    public DialogueGraphNode currentNode;
    [TextArea(20,4)]
    public string rawInput;
    public enum DialogueType { Dialogue, Choices }
    public enum Speaker { NPC, Player }

    public char tokenSeparator;

    void Start()
    {
        string[] tokens = rawInput.Split(tokenSeparator, StringSplitOptions.RemoveEmptyEntries);
        nodes = new List<DialogueGraphNode>();

        bool inNode = false;
        bool inDialogue = false;
        bool inChoices = false;

        bool nodeIdFound = false;
        int nodeId = -1;

        bool speakerIdentified = false;
        Speaker speaker;

        bool lookingForChoice = true;

        List<DialogueLine> lineList = new List<DialogueLine>();
        DialogueLine currentLine = (DialogueLine)ScriptableObject.CreateInstance(typeof(DialogueLine));


        DialogueGraphNode d = ScriptableObject.CreateInstance<DialogueGraphNode>();
        foreach(string t in tokens)
        {
            string token = t.Trim();
            Debug.Log(token);

            if(token == "(")
            {
                inNode = true;
                continue;
            }
            if(token == ")")
            {
                inNode = false;
                continue;
            }
            if(inNode)
            {
                if(!nodeIdFound)
                {
                    nodeId = int.Parse(token);
                    nodeIdFound = true;
                    continue;
                }
                if(token == "[")
                {
                    inDialogue = true;
                    continue;
                    // d.lineList = new string[] {"0","2"};
                    // nodes.Add(d);
                    // Debug.Log("dialogue start");
                }
                if(token == "]")
                {
                    inDialogue = false;

                    continue;
                    // rootNode = nodes[0];
                    // Debug.Log("dialogue end");
                }
                if(inDialogue)
                {
                    if(token == "{")
                    {
                        inChoices = true;
                        continue;
                    }
                    if(token == "}")
                    {
                        inChoices = false;
                        continue;
                    }
                    currentLine.line = token;
                }
            }

        }
    }
}
