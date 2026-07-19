using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableKVP<TKey, TValue>
{
    public TKey key;
    public TValue value;
    public SerializableKVP(TKey k, TValue v) { key = k; value = v; }
}

[System.Serializable]
public class RoomSaveData
{
    public string roomID;
    public List<string> collectedInteractables = new();
    public List<string> collectedCollectables = new();
    public List<SerializableKVP<string, int>> npcStates = new();
    public List<SerializableKVP<string, int>> bushStates = new();
}

[System.Serializable]
public class SaveData
{
    public int currency; //
    public List<string> questItems = new(); //
    public List<bool> claimedTools = new(); //
    public List<string> completedPurchases = new(); //
    public List<int> collectedMaps = new(); //
    public int maxGrappleCharges; //
    public float grappleLength; //
    public List<string> worldIDs = new(); //
    public List<RoomSaveData> rooms = new(); //
    public string lastSavePointID;
    public string lastSavedRoomID;
    public int lastSavedRegion;

    public int saveVersion = 1;
}
