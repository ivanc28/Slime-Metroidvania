#if UNITY_EDITOR
using System.Collections;
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
    [Tooltip("How long the player must wait before being able to enter a new transition")]
    public float transitionCooldown;
    public float moveToTransitionSpeed;
    public enum Direction { Horizontal, Vertical }
    public Direction direction;

    private bool canTransition = false;
    private bool canMoveToTransitionGap = false;
    private void Start()
    {
        StartCoroutine(EnableTransitionAfterDelay());
    }
    private void Update()
    {
        if (canMoveToTransitionGap)
        {
            Vector2 targetPos = GetMoveToTransPoint(direction, Player.Instance.transform.position, transform.position);
            Player.Instance.transform.position = Vector2.MoveTowards(Player.Instance.transform.position, targetPos, Time.deltaTime * moveToTransitionSpeed);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canTransition)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(MoveToTransitionPoint());
                canTransition = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canTransition)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(MoveToTransitionPoint());
                canTransition = false;
            }
        }
    }
    private IEnumerator MoveToTransitionPoint()
    {
        Player.Instance.EnableMovement(false);
        Player.Instance.rb.linearVelocity = Vector2.zero;
        canMoveToTransitionGap = true;
        yield return GameManager.Instance.GoNextRoom(nextRoomID, nextSpawnPointID, fadeTime);
    }
    private IEnumerator EnableTransitionAfterDelay()
    {
        yield return new WaitForSeconds(transitionCooldown);
        canTransition = true;
    }
    private Vector2 GetMoveToTransPoint(Direction dir, Vector2 playerPos, Vector2 transitionPos)
    {
        if(dir == Direction.Vertical)
        {
            return new Vector2(playerPos.x, transitionPos.y);
        }
        else
        {
            return new Vector2(transitionPos.x, playerPos.y);
        }
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, $"Next Spawn Point: {nextSpawnPointID}");
    }
    #endif
}
