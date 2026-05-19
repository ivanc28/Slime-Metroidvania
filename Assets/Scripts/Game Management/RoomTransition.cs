#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    public string nextRoomID;
    public string nextSpawnPointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GoNextRoom(nextRoomID, nextSpawnPointID);
        }
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, $"Next Spawn Point: {nextSpawnPointID}");
    }
    #endif
}
