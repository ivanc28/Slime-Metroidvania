using UnityEngine;

public class FastTravel : MonoBehaviour
{
    public string fastTravelID;
    public string roomNameToTP;
    public float fadeTime;
    private bool isUnlocked;
    [SerializeField] Transform popoutPos;

    private void Start()
    {
        isUnlocked = GameManager.Instance.fastTravelIDs.Contains(fastTravelID);
    }
    public void GoIn()
    {

    }

    public void PopOut()
    {
        Player.Instance.transform.position = popoutPos.position;
    }
    public void UnlockFastTravel()
    {
        GameManager.Instance.fastTravelIDs.Add(fastTravelID);
        isUnlocked = true;
    }
}
