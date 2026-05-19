#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, $"This Spawn Point: {spawnPointID}");
    }
    #endif
}
