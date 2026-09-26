using UnityEngine;

[System.Serializable]
public class DialogueOption : ScriptableObject
{
    public string choiceText;
    public int nextNodeId;
    public DialogueGraphNode nextNode;
    public DialogueEvent[] dialogueEvents;
    public Condition[] conditions;
}