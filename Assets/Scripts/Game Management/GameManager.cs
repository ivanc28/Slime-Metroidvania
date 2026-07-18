using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    private string lastSavedRoomID;
    private Region lastSavedRegion;
    private string lastSavePointID;
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
    public string GetCurrRoomID()
    {
        return currRoomID;
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
        SaveGame();
        Destroy(gameObject);
    }
    public void SetSavePointData(string savedRoomID, Region savedRegion, string savePointID)
    {
        lastSavedRoomID = savedRoomID;
        lastSavedRegion = savedRegion;
        lastSavePointID = savePointID;
    }
    public string GetLastSavedRoomID()
    {
        return lastSavedRoomID;
    }
    public SaveData BuildSaveData()
    {
        SaveData save = new();
        save.currency = Player.Instance.currencyData.GetCurrency();
        save.maxGrappleCharges = Player.Instance.maxGrappleCharges;
        save.grappleLength = Player.Instance.GetGrappleLength();
        save.questItems = Player.Instance.inventory.GetAllItems().Select(item => item.itemName).ToList();
        save.worldIDs = worldIDs.ToList();
        save.collectedMaps = Player.Instance.inventory.GetCollectedMaps().Select(x => (int)x).ToList();
        save.claimedTools = Player.Instance.tools.GetClaimedTools().ToList();
        save.completedPurchases = Player.Instance.inventory.GetCompletedPurchases();
        save.lastSavedRoomID = lastSavedRoomID;
        save.lastSavedRegion = (int)lastSavedRegion;
        save.lastSavePointID = lastSavePointID;
        foreach(var kva in roomStates)
        {
            RoomData room = kva.Value;
            RoomSaveData roomSave = new();
            roomSave.roomID = kva.Key;
            roomSave.collectedInteractables = room.collectedInteractables.ToList();
            roomSave.collectedCollectables = room.collectedCollectables.ToList();
            foreach (var npc in room.npcStates)
                roomSave.npcStates.Add(new SerializableKVP<string, int>(npc.Key, npc.Value));
            foreach (var bush in room.bushStates)
                roomSave.bushStates.Add(new SerializableKVP<string, int>(bush.Key, bush.Value));
            save.rooms.Add(roomSave);
        }
        return save;
    }
    public void ApplySaveData(SaveData save)
    {
        Player.Instance.currencyData.SetCurrencyOnLoad(save.currency);
        Player.Instance.maxGrappleCharges = save.maxGrappleCharges;
        Player.Instance.SetGrappleLengthOnLoad(save.grappleLength);
        worldIDs = new HashSet<string>(save.worldIDs);
        Player.Instance.inventory.SetCompletedPurchasesOnLoad(save.completedPurchases);
        lastSavedRoomID = save.lastSavedRoomID;
        lastSavePointID = save.lastSavePointID;
        lastSavedRegion = (Region)save.lastSavedRegion;
        currRegion = lastSavedRegion;
        for(int toolIndex = 0; toolIndex < save.claimedTools.Count; toolIndex++)
        {
            if (save.claimedTools[toolIndex])
            {
                Player.Instance.tools.ClaimTool((PlayerTools.Tool)toolIndex);
            }
        }
        foreach(int mapRegion in save.collectedMaps)
        {
            Player.Instance.inventory.CollectMap((Region)mapRegion);
        }
        foreach(RoomSaveData roomSave in save.rooms)
        {
            AddRoom(roomSave.roomID);
            RoomData room = roomStates[roomSave.roomID];
            room.collectedInteractables = new HashSet<string>(roomSave.collectedInteractables);
            room.collectedCollectables = new HashSet<string>(roomSave.collectedCollectables);
            foreach(var kvp in roomSave.npcStates)
            {
                room.npcStates[kvp.key] = kvp.value;
            }
            foreach(var kvp in roomSave.bushStates)
            {
                room.bushStates[kvp.key] = kvp.value;
            }
        }
    }
    public void SaveGame()
    {
        SaveData saveData = BuildSaveData();
        SaveSystem.Save(saveData);
    }
}
