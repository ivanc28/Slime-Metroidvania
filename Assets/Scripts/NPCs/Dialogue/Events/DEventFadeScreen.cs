using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeScreenEvent", menuName = "ScriptableData/Dialogue/Event/UI/FadeScreenEvent")]
public class DEventFadeScreen : DialogueEvent
{
    public float fadeTime;
    public float blackTime;
    public override void Invoke()
    {
        
    }
    public override IEnumerator InvokeAndWait(MonoBehaviour runner)
    {
        UIManager.Instance.FadeOut();
        yield return new WaitForSeconds(fadeTime);
        yield return new WaitForSeconds(blackTime);
        UIManager.Instance.FadeIn();
        yield return new WaitForSeconds(fadeTime);
    }
}
