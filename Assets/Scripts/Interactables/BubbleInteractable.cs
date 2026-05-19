using System.Collections;
using UnityEngine;

public class BubbleInteractable : Interactables
{
    private BubbleObj bubbleGrappleObj;
    public override void MakeStart()
    {
        base.MakeStart();
        bubbleGrappleObj = transform.parent.gameObject.GetComponent<BubbleObj>();
    }
    public override void OnInteract()
    {
        if (bubbleGrappleObj.IsHookAttached() && Player.Instance.GetIsLocked())
        {
            if (data.destroyOnInteract)
            {
                SpawnPebbles(numPebbles);
            }
            else
            {
                StartCoroutine(PopBubble(bubbleGrappleObj.popTime));
            }
        }
    }

    private IEnumerator PopBubble(float delayBeforeReappear)
    {
        bubbleGrappleObj.DeactivateBubble();
        yield return new WaitForSeconds(delayBeforeReappear);
        bubbleGrappleObj.ActivateBubble();
    }
}
