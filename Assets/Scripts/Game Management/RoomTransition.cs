#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    [Tooltip("Just the next room's name, like \"SampleScene\"")]
    public string nextRoomID;
    [Tooltip("The spawn point we teleport to in the next room")]
    public string nextSpawnPointID;
    [Tooltip("How much time we wait for the screen to fade before transitioning (in seconds)")]
    public float fadeTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(GameManager.Instance.GoNextRoom(nextRoomID, nextSpawnPointID, fadeTime));
            Player.Instance.EnableMovement(false);
            Player.Instance.rb.linearVelocity = Vector2.zero;
        }
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, $"Next Spawn Point: {nextSpawnPointID}");
    }
    #endif
}
