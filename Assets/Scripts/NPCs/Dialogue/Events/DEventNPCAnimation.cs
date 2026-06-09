using UnityEngine;

[CreateAssetMenu(fileName = "NPCAnimation", menuName = "ScriptableData/Dialogue/Event/NPCAnimation")]
public class DEventNPCAnimation : DialogueEvent
{
    [Tooltip("To use trigger name, check this box")]
    public bool useTrigger;
    [Tooltip("To use bool name, check this box")]
    public bool useBool;
    public string triggerName;
    public string boolName;
    public bool boolValue;

    public override void Invoke()
    {
        if (NPC.CurrentInteractingNPC != null)
        {
            NPC npc = NPC.CurrentInteractingNPC;
            if (useTrigger)
            {
                npc.anim.SetTrigger(triggerName);
            }
            if (useBool)
            {
                npc.anim.SetBool(boolName, boolValue);
            }
        }
    }
}
