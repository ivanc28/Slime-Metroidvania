using UnityEngine;

public class MapCollectable : Collectable
{
    public GameManager.Region region;
    public override void Collect()
    {
        Player.Instance.inventory.CollectMap(region);
    }
}
