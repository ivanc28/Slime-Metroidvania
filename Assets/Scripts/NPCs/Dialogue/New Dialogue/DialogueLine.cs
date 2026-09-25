using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueTextPart", menuName = "Scriptable Objects/DialogueTextPart")]
public class DialogueLine : ScriptableObject
{
    public enum Speaker { NPC, Player }
    public Speaker speaker;
    [Tooltip("A one liner. This should be EMPTY if there is more than 1 choice")]
    [TextArea(2,4)]
    public string line;
    [Tooltip("0 choices means this is the last line. 1 choice means no choices (i.e. pick this one), go to next line. 2+ choices means we give the player dialogue options")]
    public List<DialogueChoice> choices;
}
