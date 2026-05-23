using UnityEngine;

public class ToolCollectable : Collectable
{
    public PlayerTools.Tool tool;
    public override void Collect()
    {
        Player.Instance.tools.ClaimTool(tool);
    }
}
