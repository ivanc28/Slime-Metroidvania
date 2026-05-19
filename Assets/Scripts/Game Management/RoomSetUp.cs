using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomSetUp : MonoBehaviour
{
    [SerializeField] SpawnPoint[] spawnPoints;
    string roomID;
    private void Awake()
    {
        roomID = SceneManager.GetActiveScene().name;
        GameManager.Instance.AddRoom(roomID);
        GameManager.Instance.SetCurrRoomID(roomID);
    }
    private void Start()
    {
        FindFirstObjectByType<CinemachineCamera>().Follow = Player.Instance.transform;
        string spawnPointID = GameManager.Instance.GetNextSpawnPointID();
        if(spawnPointID == null || spawnPointID == string.Empty)
        {
            return;
        }
        bool foundScene = false;
        foreach(SpawnPoint point in spawnPoints)
        {
            if(point.spawnPointID == spawnPointID)
            {
                foundScene = true;
                Player.Instance.transform.position = point.transform.position;
                Input.ResetInputAxes();
            }
        }
        if (!foundScene)
        {
            Debug.LogWarning($"Failed to find spawnPointID labeled {spawnPointID}");
        }
    }    
}
