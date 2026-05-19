using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Dictionary<string, RoomData> roomStates = new();

    private string currRoomID;
    private string nextSpawnPointID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TESTING ONLY
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void AddRoom(string room)
    {
        if (!roomStates.ContainsKey(room))
        {
            roomStates.Add(room, new RoomData());
        }
    }
    public RoomData GetRoomData(string room)
    {
        return roomStates[room];
    }
    public RoomData GetCurrRoomData()
    {
        return roomStates[currRoomID];
    }
    public void SetCurrRoomID(string id)
    {
        currRoomID = id;
    }
    public string GetNextSpawnPointID()
    {
        return nextSpawnPointID;
    }
    public void GoNextRoom(string roomID, string spawnPointID)
    {
        nextSpawnPointID = spawnPointID;
        SceneManager.LoadScene(roomID);
    }

}
