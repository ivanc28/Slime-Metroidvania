using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;
    [Tooltip("The default sprite if the item has no sprite")]
    public Sprite defaultItemSprite;
    public void Initialize(QuestCollectableData item)
    {
        if(item.sprite != null)
        {
            itemImage.sprite = item.sprite;
        }
        else
        {
            itemImage.sprite = defaultItemSprite;
        }
    }
    public void Initialize(ToolInventoryData tool)
    {
        if (tool.sprite != null)
        {
            itemImage.sprite = tool.sprite;
        }
        else
        {
            itemImage.sprite = defaultItemSprite;
        }
    }
}
