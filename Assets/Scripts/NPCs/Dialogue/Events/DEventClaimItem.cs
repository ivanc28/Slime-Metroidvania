using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ClaimItemEvent", menuName = "ScriptableData/Dialogue/Event/ClaimItemEvent")]
public class DEventClaimItem : DialogueEvent
{
    public QuestCollectableData questItem;
    public int optionalPebbleCost = 0;
    public string claimMessage;
    public override void Invoke()
    {
        if (optionalPebbleCost > 0)
        {
            if (Player.Instance.currencyData.OnPurchase(optionalPebbleCost))
            {
                Player.Instance.inventory.AddQuestItem(questItem);
            }
        }
        else
        {
            Player.Instance.inventory.AddQuestItem(questItem);
        }
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        Invoke();
        yield return runner.StartCoroutine(UIManager.Instance.ShowCollectTextAndWait(claimMessage));
    }
}
