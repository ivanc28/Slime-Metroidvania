using UnityEngine;

public class WhiskInteractable : Interactables
{
    public WhiskWall wall;
    public override void OnInteract()
    {
        if(wall != null)
        {
            wall.CallMoveCoroutine();
        }
    }
}
