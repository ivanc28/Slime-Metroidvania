using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Dictionary<string, RoomData> roomStates = new();
    public HashSet<string> worldIDs = new();

    private string currRoomID;
    private string nextSpawnPointID;
    public bool GamePaused { get; private set; }
    public enum Region { None, Sweet, Savory, Sour, Bitter, Salty }
    private Region currRegion;
    private float currTimeScale = 1f;

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

    // Update is called once per frame
    void Update()
    {
        // TESTING ONLY
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
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
    public IEnumerator GoNextRoom(string roomID, string spawnPointID, float fadeTime)
    {
        UIManager.Instance.FadeOut();
        yield return new WaitForSeconds(fadeTime);
        nextSpawnPointID = spawnPointID;
        SceneManager.LoadScene(roomID);
    }
    public void SetCurrRegion(Region region)
    {
        currRegion = region;
    }
    public Region GetCurrRegion()
    {
        return currRegion;
    }
    public void SetPaused(bool paused)
    {
        if (paused)
        {
            currTimeScale = Time.timeScale;
        }
        GamePaused = paused;
        Time.timeScale = paused ? 0 : currTimeScale;
    }

    public void ResetGame()
    {
        if(UIManager.Instance != null)
        {
            Destroy(UIManager.Instance.gameObject);
        }
        if(Player.Instance != null)
        {
            Destroy(Player.Instance.gameObject);
        }
        if(MusicManager.Instance != null)
        {
            Destroy(MusicManager.Instance.gameObject);
        }
        SetPaused(false);
        Destroy(gameObject);
    }
}
