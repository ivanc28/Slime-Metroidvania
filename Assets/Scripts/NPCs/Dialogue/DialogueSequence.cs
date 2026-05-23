using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableData/Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    [Tooltip("The first dialogue node of the sequence")]
    public DialogueNode rootNode;
    [Tooltip("A condition that must be met to go to the next sequence once THIS sequence is done")]
    public Condition nextSequenceCondition;
    [Tooltip("A condition that, if met, skips THIS sequence")]
    public Condition skipSequenceCondition;
}
