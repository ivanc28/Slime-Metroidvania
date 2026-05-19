using System;
using UnityEngine;

public class PlayerTools
{
    public enum Tool { None, Spoon, Fork, Chopsticks, Umbrella, Scissors, Whisk, BubbleBlower};
    private Tool currTool;
    private bool[] claimedTools = new bool[Enum.GetValues(typeof(Tool)).Length];
    public PlayerTools()
    {
        currTool = Tool.None;
    }
    public void ClaimTool(Tool tool)
    {
        claimedTools[(int)tool] = true;
        Player.Instance.UnlockTool(tool);
    }
    public void SwapTool(Tool tool)
    {
        if (claimedTools[(int)tool])
        {
            currTool = tool;
            Debug.Log($"Swapped to tool: {tool}");
        }
    }
    public bool HasTool(Tool tool)
    {
        return claimedTools[(int)tool];
    }
    public Tool GetCurrTool()
    {
        return currTool;
    }

}
