#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;
    public enum Direction { Up, Down, Left, Right }
    public Direction spawnDirection;
    [Tooltip("If spawn direction is Up, then if this is checked, go to the right, otherwise go to the left")]
    public bool upRight;
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, $"This Spawn Point: {spawnPointID}");
    }
    #endif
}
