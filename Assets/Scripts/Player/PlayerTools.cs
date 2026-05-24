using System;
using UnityEngine;

public class PlayerTools
{
    public enum Tool { None, Spoon, Fork, Chopsticks, Umbrella, Scissors, Whisk, BubbleBlower};
    private Tool currTool;
    private bool[] claimedTools = new bool[Enum.GetValues(typeof(Tool)).Length];
    public ToolOption currToolOption;
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
    public ToolOption GetCurrToolOption()
    {
        return currToolOption;
    }
    /// <summary>
    /// FOR UI ONLY
    /// </summary>
    /// <param name="option"></param>
    public void SetCurrToolOption(ToolOption option)
    {
        currToolOption = option;
    }

}
