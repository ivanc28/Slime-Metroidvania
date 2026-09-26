using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueTextPart", menuName = "Scriptable Objects/DialogueTextPart")]
public class DialogueLine : ScriptableObject
{
    public enum Speaker { NPC, Player }
    public Speaker speaker;

    public string line;
    public List<DialogueOption> choices;
}
