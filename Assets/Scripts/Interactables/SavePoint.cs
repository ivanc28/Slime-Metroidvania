using System.Collections;
using UnityEngine;

public class SavePoint : Inspectable
{
    public string savePointID;
    public KeyCode interactKey;

    public override IEnumerator Inspect(Player player)
    {
        Player.Instance.InInteraction = true;
        GameManager.Instance.SetSavePointData(GameManager.Instance.GetCurrRoomID(), GameManager.Instance.GetCurrRegion(), savePointID);
        yield return null;
        GameManager.Instance.SaveGame();
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSaveIcon();
        }
        Player.Instance.InInteraction = false;
    }
}
