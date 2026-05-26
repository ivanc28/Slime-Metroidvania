using System;
using UnityEngine;

public class PlayerTools
{
    public enum Tool { None, Spoon, Fork, Chopsticks, Umbrella, Scissors, Whisk, BubbleBlower};
    private Tool currTool;
    private bool[] claimedTools = new bool[Enum.GetValues(typeof(Tool)).Length];
    [HideInInspector] public ToolOption currToolOption;
    public PlayerTools()
    {
        currTool = Tool.None;
    }
    public void ClaimTool(Tool tool)
    {
        claimedTools[(int)tool] = true;
        Player.Instance.UnlockTool(tool);
        if(tool == Tool.None)
        {
            return;
        }
        foreach(ToolInventoryData toolData in Player.Instance.data.toolUIData)
        {
            if(toolData.tool == tool)
            {
                Player.Instance.inventory.AddToolItem(toolData);
                break;
            }
        }
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
