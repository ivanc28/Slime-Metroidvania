using UnityEngine;

public class BubbleObj : GrappleObj
{
    public BubbleInteractable bubbleInteractGain;
    public float popTime;
    public override void EffectOnPlayerContactAfterHook()
    {
        // do nothing
    }

    public void ActivateBubble()
    {
        objCollider.enabled = true;
        objRenderer.enabled = true;
    }
    public void DeactivateBubble()
    {
        DetachHook();
        objCollider.enabled = false;
        objRenderer.enabled = false;
    }
}
