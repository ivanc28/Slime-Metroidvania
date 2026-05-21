using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableData/Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueNode rootNode;
    public Condition nextSequenceCondition;
}
