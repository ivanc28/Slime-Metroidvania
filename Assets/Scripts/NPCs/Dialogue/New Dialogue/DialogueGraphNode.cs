using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public class DialogueGraphNode : ScriptableObject
{
    // public enum Speaker { NPC, Player }
    // public Speaker speaker;
    public int nodeId;
    public string folderPath;
    [TextArea(5,4)]
    public string rawInput = "";
    public bool hasChoices;
    public int nextNodeId;
    public DialogueGraphNode nextNode;
    public Condition[] continueConditions;

    public List<Line> lineList;

    // public void fillNode(Dictionary<int,DialogueGraphNode> nodes)
    // {
    //     if(nodeId == -1)
    //     {

    //     }
    // }
    public void ParseInput()
    {
        Debug.Log(folderPath);
        string[] lineTokens = rawInput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if(lineTokens.Length == 0)
        {
            return;
        }
        AssetDatabase.DeleteAsset($"{folderPath}/Lines");
        AssetDatabase.CreateFolder($"{folderPath}", "Lines");
        // Debug.Log(lineTokens[0]);
        nodeId = int.Parse(lineTokens[0].Trim());
        foreach(string l in lineTokens.Skip(1))
        {
            // Debug.Log(l);
            string[] tokens = l.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if(tokens[0].Trim().Equals("D"))
            {
                Debug.Log("Dialogue");
                DialogueLine line = ScriptableObject.CreateInstance<DialogueLine>();
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/Lines/DialogueLine.asset");
                AssetDatabase.CreateAsset(line, uniqueFileName);
                if(tokens[1].Trim().Equals("P"))
                {
                    line.speaker = DialogueLine.Speaker.Player;
                }
                else if(tokens[0].Trim().Equals("N"))
                {
                    line.speaker = DialogueLine.Speaker.NPC;
                }
                line.text = tokens[2];
            }
            else if(tokens[0].Trim().Equals("C"))
            {
                Debug.Log("Choice");
            }
            else if(tokens[0].Trim().Equals("E"))
            {
                Debug.Log("Event");
            }
        }
    }
}
