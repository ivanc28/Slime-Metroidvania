
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("If there are at least 2 choices, give them names")]
    public string choiceText;
    [Tooltip("The next dialogue line")]
    public DialogueNode nextNode;
    [Tooltip("Optional events that get called when this choice is chosen")]
    public DialogueEvent[] dialogueEvents;
    [Tooltip("A condition that must be met in order to select this choice")]
    public Condition condition;
}
