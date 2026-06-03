using System.Collections;
using UnityEngine;

public abstract class DialogueEvent : ScriptableObject
{
    public abstract void Invoke();
    public virtual IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        Debug.Log("DEventClaimTool InvokeAndWait called");
        Invoke();
        yield break;
    }
}
