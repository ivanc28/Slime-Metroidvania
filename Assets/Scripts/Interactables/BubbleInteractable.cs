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
        Debug.Log($"{gameObject.name} tried to interact... HookAttached is {bubbleGrappleObj.IsHookAttached()} and IsLocked is {Player.Instance.GetIsLocked()}");
        if (bubbleGrappleObj.IsHookAttached() && Player.Instance.GetIsLocked())
        {
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
}
