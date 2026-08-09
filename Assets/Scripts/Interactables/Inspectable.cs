using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Inspectable : MonoBehaviour
{
    public GameObject keyIcon;
    public float inspectRange = 3f;
    public static List<Inspectable> inspectableList = new();
    public virtual Transform GetTransform()
    {
        return transform;
    }
    public virtual float GetInspectRange()
    {
        return inspectRange;
    }
    public abstract IEnumerator Inspect(Player player);
    public virtual bool CanInspect(Player player)
    {
        return player.CanInteract() && !player.GetIsLocked();
    }
    public virtual void OnFocusChanged(bool isFocued)
    {
        keyIcon.SetActive(isFocued);
    }

    private void OnEnable()
    {
        inspectableList.Add(this);
    }
    private void OnDisable()
    {
        inspectableList.Remove(this);
    }
}
