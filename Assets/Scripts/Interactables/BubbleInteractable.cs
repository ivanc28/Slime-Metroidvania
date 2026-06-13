using System.Collections;
using UnityEngine;

public class BubbleInteractable : Interactables
{
    private BubbleObj bubbleGrappleObj;
    private bool successfulInteract;
    public override void MakeStart()
    {
        base.MakeStart();
        bubbleGrappleObj = transform.parent.gameObject.GetComponent<BubbleObj>();
    }
    public override void OnInteract()
    {       
        if (bubbleGrappleObj.IsHookAttached() && Player.Instance.GetIsLocked() && Player.Instance.currentAttachedCollider == bubbleGrappleObj.objCollider)
        {
            successfulInteract = true; // only set true on a valid pop
            Player.Instance.SetGravityToFalling();
            if (data.destroyOnInteract)
            {
                SpawnPebbles(numPebbles);
            }
            else
            {
                StartCoroutine(bubbleGrappleObj.PopBubble(bubbleGrappleObj.popTime));
            }
        }
    }
    public override void BehaviourBeforeDestroy()
    {
        bubbleGrappleObj = transform.parent.gameObject.GetComponent<BubbleObj>();
        bubbleGrappleObj.bubbleInteractGain = null;
    }

    public override bool DidInteractSucceed()
    {
        return successfulInteract;
    }
}
