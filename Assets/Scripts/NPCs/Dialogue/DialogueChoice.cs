
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("If there are at least 2 choices, give them names")]
    public string choiceText;
    [Tooltip("The next dialogue line")]
    public DialogueNode nextNode;
    [Tooltip("An optional event that gets called when this choice is chosen")]
    public DialogueEvent dialogueEvent;
}
