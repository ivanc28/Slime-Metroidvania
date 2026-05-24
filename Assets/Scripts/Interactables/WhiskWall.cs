using System.Collections;
using UnityEngine;

public class WhiskWall : MonoBehaviour
{
    public WhiskInteractable interactable;
    public Transform wallPos;
    public Transform endPos;
    [Tooltip("Power of function that controls speed the wall moves from beginning to end pos")]
    public float moveTime;
    public float power;
    private float moveDistance;
    Vector2 startPos;
    Vector2 moveDir;
    bool isMoving;

    RoomData room;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        room = GameManager.Instance.GetCurrRoomData();
        if (room.collectedInteractables.Contains(interactable.GetInteractableID()))
        {
            wallPos.transform.position = endPos.position;
            return;
        }
        startPos = wallPos.transform.position;
        Vector2 vec = endPos.position - wallPos.transform.position;
        moveDir = vec.normalized;
        moveDistance = vec.magnitude;
    }

    private IEnumerator MoveToEndPos()
    {
        float time = 0;
        float percentOfWayToEnd = 0;
        while (time < moveTime)
        {
            percentOfWayToEnd = Mathf.Lerp(0, 1, Mathf.Pow(time / moveTime, power));
            wallPos.transform.position = startPos + moveDistance * percentOfWayToEnd * moveDir;
            time += Time.deltaTime;
            yield return null;
        }
        wallPos.transform.position = endPos.position;
    }

    public void CallMoveCoroutine()
    {
        if (isMoving)
        {
            return;
        }
        isMoving = true;
        StartCoroutine(MoveToEndPos());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallPos.transform.position, endPos.position - wallPos.transform.position);
    }
}
