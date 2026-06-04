using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class ToolOption : MonoBehaviour
{
    public ToolSelectionData selectionData;
    public Image toolBG;
    public Image lockIcon;
    public PlayerTools.Tool tool;
    public Image toolImage;

    private void Awake()
    {
        lockIcon.sprite = selectionData.lockSprite;
        if(toolImage != null)
        {
            toolImage.color = Color.black;
        }
    }
    private void Update()
    {
        if(Player.Instance.GetHoveredTool() == this)
        {
            EnableToolBG(true);
        }
        else
        {
            EnableToolBG(false);
        }
    }
    public void EnableToolBG(bool enabled)
    {
        toolBG.enabled = enabled;
    }
    public void RemoveLock()
    {
        lockIcon.enabled = false;
        if (toolImage != null)
        {
            toolImage.color = Color.white;
        }
    }
}
