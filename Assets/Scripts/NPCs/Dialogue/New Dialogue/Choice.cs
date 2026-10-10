using UnityEngine;

// [System.Serializable]
public class Choice : ScriptableObject
{
    public string choiceText;
    public Condition[] conditions;
    public int nextNodeId;
    public DialogueGraphNode nextNode;
    public DialogueEvent[] dialogueEvents;
}