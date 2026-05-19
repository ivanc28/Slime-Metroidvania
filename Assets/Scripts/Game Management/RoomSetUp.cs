using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSetUp : MonoBehaviour
{
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
    }
}
