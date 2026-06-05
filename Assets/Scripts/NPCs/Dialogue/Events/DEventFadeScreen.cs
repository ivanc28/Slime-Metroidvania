using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeScreenEvent", menuName = "ScriptableData/Dialogue/Event/UI/FadeScreenEvent")]
public class DEventFadeScreen : DialogueEvent
{
    [Tooltip("does NOT include the time it actually takes to fade, so take the number you wanted and add like 0.75 sec")]
    public float fadeTime;
    public override void Invoke()
    {
        
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        UIManager.Instance.FadeIn();
        yield return new WaitForSeconds(fadeTime);
        UIManager.Instance.FadeOut();
    }
}
