using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class DialogueGraph : MonoBehaviour
{
    public string conversationTitle;
    public DialogueGraphNode rootNode;
    public DialogueGraphNode currentNode;
    public List<DialogueGraphNode> nodes;
    // [TextArea(20,4)]
    public string rawInput;
    public enum DialogueType { Dialogue, Choices }
    // public enum Speaker { NPC, Player }

    public char tokenSeparator;

    // [ContextMenu("Parse Input to Nodes")]
    // void DoSomething()
    // {
    //     string[] tokens = rawInput.Split(tokenSeparator, StringSplitOptions.RemoveEmptyEntries);
    //     // Debug.Log(tokens[0]);
    //     // 1. Create the instance in memory
    //     DialogueGraphNode newNode = ScriptableObject.CreateInstance<DialogueGraphNode>();

    //     // 2. Generate a unique file path to prevent overwriting existing assets
    //     string path = "Assets/ScriptableObjects/Dialogue/Nodes/" + conversationTitle + ".asset";
    //     // path = AssetDatabase.GenerateUniqueAssetPath(path);

    //     // 3. Save the asset to the Project folder
    //     Debug.Log(path);
    //     Debug.Log(conversationTitle);
    //     bool success = AssetDatabase.DeleteAsset(path);
    //     Debug.Log(success);
    //     AssetDatabase.CreateAsset(newNode, path);
    //     AssetDatabase.SaveAssets();

    //     Debug.Log($"Asset successfully created at: {path}");

    //     nodes.Add(newNode);
    // }

    [ContextMenu("Reset Node List")]
    void ResetNodeList()
    {
        string folderName = $"{gameObject.name}_{GetInstanceID()}";
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}");
        AssetDatabase.CreateFolder("Assets/ScriptableObjects/Dialogue/ConversationNodes", folderName);
        Debug.Log(folderName);
        for(int i = 0; i < nodes.Count; i++)
        {
            // 1. Create the instance in memory
            DialogueGraphNode newNode = ScriptableObject.CreateInstance<DialogueGraphNode>();
            AssetDatabase.CreateFolder($"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}", i.ToString());
            AssetDatabase.CreateFolder($"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}", "Lines");
            
            // 2. Generate a unique file path to prevent overwriting existing assets
            string path = $"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}/{i}_node.asset";
            // path = AssetDatabase.GenerateUniqueAssetPath(path);

            // 3. Save the asset to the Project folder
            Debug.Log(path);
            Debug.Log(conversationTitle);
            AssetDatabase.CreateAsset(newNode, path);
            newNode.folderPath = $"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}";
            AssetDatabase.SaveAssets();

            Debug.Log($"Asset successfully created at: {path}");

            nodes[i] = newNode;
        }
    }

    [ContextMenu("Fill Node List")]
    void FillNodeList()
    {
        AssetDatabase.SaveAssets();
        string folderName = $"{gameObject.name}_{GetInstanceID()}";
        // if(!AssetDatabase.IsValidFolder(folderName))
        // {
        //     return;
        // }
        string[] folderGuids = AssetDatabase.FindAssets("t:folder", new[] { $"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}" });
        // Debug.Log(folderGuids);
        for(int i = 0; i < nodes.Count; i++)
        {
            string assetPath = "";
            string folderPath = "";
            if(i < folderGuids.Length)
            {
                folderPath = AssetDatabase.GUIDToAssetPath(folderGuids[i]);
                if(!folderPath.Equals(""))
                {
                    Debug.Log(folderPath);
                    Debug.Log(i);
                    string[] assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
                    assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                }
            }

            
            DialogueGraphNode asset = AssetDatabase.LoadAssetAtPath<DialogueGraphNode>(assetPath);
            
            if(asset == null)
            {
                // 1. Create the instance in memory
                DialogueGraphNode newNode = ScriptableObject.CreateInstance<DialogueGraphNode>();
                AssetDatabase.CreateFolder($"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}", i.ToString());
                AssetDatabase.CreateFolder($"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}", "Lines");
                
                // 2. Generate a unique file path to prevent overwriting existing assets
                string path = $"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}/{i}_node.asset";
                // path = AssetDatabase.GenerateUniqueAssetPath(path);

                // 3. Save the asset to the Project folder
                Debug.Log(path);
                Debug.Log(conversationTitle);
                AssetDatabase.CreateAsset(newNode, path);
                newNode.folderPath = $"Assets/ScriptableObjects/Dialogue/ConversationNodes/{folderName}/{i}";
                AssetDatabase.SaveAssets();

                Debug.Log($"Asset successfully created at: {path}");

                nodes[i] = newNode;
            }
            else
            {
                nodes[i] = asset;
            }
        }
    }

    [ContextMenu("Parse Node List")]
    void ParseNodeList()
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            nodes[i].ParseInput();
        }
    }

    void Start()
    {
        // string[] tokens = rawInput.Split(tokenSeparator, StringSplitOptions.RemoveEmptyEntries);
        // nodes = new Dictionary<int, DialogueGraphNode>();
        // rootNode = null;

        // bool inNode = false;
        // bool inDialogue = false;
        // bool inChoices = false;

        // bool nodeIdFound = false;
        // int nodeId = -1;

        // bool speakerIdentified = false;
        // DialogueLine.Speaker speaker = DialogueLine.Speaker.Player;

        // bool lookingForChoice = false;
        // bool hasChoices = false;
        // int choiceNodeId = -1;

        // List<DialogueLine> lineList = new List<DialogueLine>();
        // DialogueLine currentLine = (DialogueLine)ScriptableObject.CreateInstance(typeof(DialogueLine));
        // List<DialogueOption> choiceList = new List<DialogueOption>();
        // DialogueOption currentChoice = (DialogueOption)ScriptableObject.CreateInstance(typeof(DialogueOption));


        // DialogueGraphNode d = (DialogueGraphNode)ScriptableObject.CreateInstance<DialogueGraphNode>();
        // foreach(string t in tokens)
        // {
        //     string token = t.Trim();
        //     // Debug.Log(token);

        //     if(token == "(")
        //     {
        //         inNode = true;
        //         continue;
        //     }
        //     if(token == ")")
        //     {
        //         if(rootNode == null)
        //         {
        //             rootNode = d;
        //         }
        //         inNode = false;
        //         nodeIdFound = false;
        //         d.lineList = lineList;
        //         lineList = new List<DialogueLine>();
        //         // Debug.Log(nodeId);
        //         nodes.Add(nodeId, d);
        //         d.hasChoices = hasChoices;
        //         hasChoices = false;
        //         d = (DialogueGraphNode)ScriptableObject.CreateInstance<DialogueGraphNode>();
        //         continue;
        //     }
        //     if(inNode)
        //     {
        //         if(!nodeIdFound)
        //         {
        //             d.nodeId = int.Parse(token);
        //             nodeId = int.Parse(token);
        //             nodeIdFound = true;
        //             continue;
        //         }
        //         if(token == "[")
        //         {
        //             inDialogue = true;
        //             // Debug.Log("dialogue start");
        //             continue;
        //             // d.lineList = new string[] {"0","2"};
        //             // nodes.Add(d);
        //         }
        //         if(token == "]")
        //         {
        //             inDialogue = false;
        //             speakerIdentified = false;
        //             // lineList.Add(currentLine);
        //             currentLine = (DialogueLine)ScriptableObject.CreateInstance(typeof(DialogueLine));
        //             // Debug.Log("dialogue end");
        //             continue;
        //             // rootNode = nodes[0];
        //         }
        //         if(inDialogue)
        //         {
        //             if(!speakerIdentified)
        //             {
        //                 if(token == "P")
        //                 {
        //                     speaker = DialogueLine.Speaker.Player;
        //                 }
        //                 if(token == "N")
        //                 {
        //                     speaker = DialogueLine.Speaker.NPC;
        //                 }
        //                 speakerIdentified = true;
        //                 continue;
        //             }
        //             if(token == "{")
        //             {
        //                 inChoices = true;
        //                 hasChoices = true;
        //                 continue;
        //             }
        //             if(token == "}")
        //             {
        //                 inChoices = false;
        //                 if(choiceList.Count > 1)
        //                 {
        //                     currentLine.choices = choiceList;
        //                 }
        //                 else
        //                 {
        //                     d.nextNodeId = choiceNodeId;
        //                     hasChoices = false;
        //                 }
        //                 choiceList = new List<DialogueOption>();
        //                 continue;
        //             }
        //             if(inChoices)
        //             {
        //                 if(lookingForChoice)
        //                 {
        //                     currentChoice.choiceText = token;
        //                     lookingForChoice = false;
        //                 }
        //                 else
        //                 {
        //                     choiceNodeId = int.Parse(token);
        //                     currentChoice.nextNodeId = choiceNodeId;
        //                     choiceList.Add(currentChoice);
        //                     currentChoice = (DialogueOption)ScriptableObject.CreateInstance(typeof(DialogueOption));
        //                     lookingForChoice = true;
        //                 }
        //                 continue;
        //             }
        //             // Debug.Log(token);
        //             currentLine.line = token;
        //             currentLine.speaker = speaker;
        //             lineList.Add(currentLine);
        //         }
        //     }
        // }
        // fillNodes(nodes);
        // // Debug.Log("start");
        // foreach(var pair in nodes)
        // {
        //     // Debug.Log(pair.Key.ToString());
        //     foreach(DialogueLine l in pair.Value.lineList)
        //     {
        //         // Debug.Log(l.line);
        //     }
        // }
    }

    // public void fillNodes(Dictionary<int,DialogueGraphNode> nodes)
    // {
    //     foreach(var pair in nodes)
    //     {
    //         DialogueGraphNode node = pair.Value;
    //         if(node.hasChoices)
    //         {
    //             DialogueLine line = node.lineList[node.lineList.Count - 1];
    //             foreach(DialogueOption choice in line.choices)
    //             {
    //                 choice.nextNode = nodes[choice.nextNodeId];
    //             }
    //         }
    //         else
    //         {
    //             node.nextNode = nodes[node.nextNodeId];
    //         }
    //     }
    // }
}
