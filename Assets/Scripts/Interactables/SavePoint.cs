using System.Collections;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public string savePointID;
    public KeyCode interactKey;
    public float interactRange;
    public GameObject keyIcon;
    private bool inRange;

    private void Update()
    {
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= interactRange;
        if (inRange)
        {
            bool canInteract = !Player.Instance.InInteraction && !Player.Instance.GetIsAttaching() && Player.Instance.GetIsGrounded() && !Player.Instance.GetIsLocked() && !Player.Instance.GetHookBeingThrown() && !Player.Instance.IsInBubble();
            if (canInteract)
            {
                keyIcon.SetActive(true);
                if (Input.GetKeyDown(interactKey))
                {
                    Player.Instance.InInteraction = true;
                    StartCoroutine(OnUse());
                }
            }
        }
        else
        {
            keyIcon.SetActive(false);
        }
    }

    private IEnumerator OnUse()
    {
        GameManager.Instance.SetSavePointData(GameManager.Instance.GetCurrRoomID(), GameManager.Instance.GetCurrRegion(), savePointID);
        yield return null;
        GameManager.Instance.SaveGame();
        Player.Instance.InInteraction = false;
    }
}
