using UnityEngine;
using System.Collections.Generic;

// [CreateAssetMenu(fileName = "DialogueTextPart", menuName = "Scriptable Objects/DialogueTextPart")]
public class DialogueLine : Line
{
    public enum Speaker { NPC, Player }
    public Speaker speaker;

    public string text;
}
