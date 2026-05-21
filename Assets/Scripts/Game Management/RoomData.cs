using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public HashSet<string> collectedTools = new();
    public HashSet<string> collectedInteractables = new();
    public HashSet<string> openedChests = new();
    public Dictionary<string, int> npcStates = new();
}
