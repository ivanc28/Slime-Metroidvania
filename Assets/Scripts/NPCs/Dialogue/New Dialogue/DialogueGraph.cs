using UnityEngine;
using System.Collections.Generic;
using System;

public class DialogueGraph : MonoBehaviour
{
    public DialogueGraphNode rootNode;
    public DialogueGraphNode currentNode;
    public Dictionary<int, DialogueGraphNode> nodes;
    [TextArea(20,4)]
    public string rawInput;
    public enum DialogueType { Dialogue, Choices }
    // public enum Speaker { NPC, Player }

    public char tokenSeparator;

    void Start()
    {
        string[] tokens = rawInput.Split(tokenSeparator, StringSplitOptions.RemoveEmptyEntries);
        nodes = new Dictionary<int, DialogueGraphNode>();

        bool inNode = false;
        bool inDialogue = false;
        bool inChoices = false;

        bool nodeIdFound = false;
        int nodeId = -1;

        bool speakerIdentified = false;
        DialogueLine.Speaker speaker = DialogueLine.Speaker.Player;

        bool lookingForChoice = false;
        bool hasChoices = false;
        int choiceNodeId = -1;

        List<DialogueLine> lineList = new List<DialogueLine>();
        DialogueLine currentLine = (DialogueLine)ScriptableObject.CreateInstance(typeof(DialogueLine));
        List<DialogueOption> choiceList = new List<DialogueOption>();
        DialogueOption currentChoice = (DialogueOption)ScriptableObject.CreateInstance(typeof(DialogueOption));


        DialogueGraphNode d = (DialogueGraphNode)ScriptableObject.CreateInstance<DialogueGraphNode>();
        foreach(string t in tokens)
        {
            string token = t.Trim();
            // Debug.Log(token);

            if(token == "(")
            {
                inNode = true;
                continue;
            }
            if(token == ")")
            {
                inNode = false;
                nodeIdFound = false;
                d.lineList = lineList;
                lineList = new List<DialogueLine>();
                Debug.Log(nodeId);
                nodes.Add(nodeId, d);
                d.hasChoices = hasChoices;
                hasChoices = false;
                d = (DialogueGraphNode)ScriptableObject.CreateInstance<DialogueGraphNode>();
                continue;
            }
            if(inNode)
            {
                if(!nodeIdFound)
                {
                    d.nodeId = int.Parse(token);
                    nodeId = int.Parse(token);
                    nodeIdFound = true;
                    continue;
                }
                if(token == "[")
                {
                    inDialogue = true;
                    Debug.Log("dialogue start");
                    continue;
                    // d.lineList = new string[] {"0","2"};
                    // nodes.Add(d);
                }
                if(token == "]")
                {
                    inDialogue = false;
                    speakerIdentified = false;
                    // lineList.Add(currentLine);
                    currentLine = (DialogueLine)ScriptableObject.CreateInstance(typeof(DialogueLine));
                    Debug.Log("dialogue end");
                    continue;
                    // rootNode = nodes[0];
                }
                if(inDialogue)
                {
                    if(!speakerIdentified)
                    {
                        if(token == "P")
                        {
                            speaker = DialogueLine.Speaker.Player;
                        }
                        if(token == "N")
                        {
                            speaker = DialogueLine.Speaker.NPC;
                        }
                        speakerIdentified = true;
                        continue;
                    }
                    if(token == "{")
                    {
                        inChoices = true;
                        hasChoices = true;
                        continue;
                    }
                    if(token == "}")
                    {
                        inChoices = false;
                        if(choiceList.Count > 1)
                        {
                            currentLine.choices = choiceList;
                        }
                        else
                        {
                            d.nextNodeId = choiceNodeId;
                            hasChoices = false;
                        }
                        choiceList = new List<DialogueOption>();
                        continue;
                    }
                    if(inChoices)
                    {
                        if(lookingForChoice)
                        {
                            currentChoice.choiceText = token;
                            lookingForChoice = false;
                        }
                        else
                        {
                            choiceNodeId = int.Parse(token);
                            currentChoice.nextNodeId = choiceNodeId;
                            choiceList.Add(currentChoice);
                            currentChoice = (DialogueOption)ScriptableObject.CreateInstance(typeof(DialogueOption));
                            lookingForChoice = true;
                        }
                        continue;
                    }
                    Debug.Log(token);
                    currentLine.line = token;
                    currentLine.speaker = speaker;
                    lineList.Add(currentLine);
                }
            }
        }
        rootNode = nodes[4];
        Debug.Log("start");
        foreach(var pair in nodes)
        {
            Debug.Log(pair.Key.ToString());
            foreach(DialogueLine l in pair.Value.lineList)
            {
                Debug.Log(l.line);
            }
        }
    }
}
