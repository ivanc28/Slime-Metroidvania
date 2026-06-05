using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ClaimZipline", menuName = "ScriptableData/Dialogue/Event/ClaimZipline")]
public class DEventClaimZipline : DEventClaimItem
{
    public override void Invoke()
    {
        base.Invoke();
        Player.Instance.ClaimZiplineStrap();
    }
}
