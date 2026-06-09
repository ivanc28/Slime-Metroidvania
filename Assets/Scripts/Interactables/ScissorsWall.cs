using UnityEngine;

public class ScissorsWall : Interactables
{
    public AudioClip[] leavesClips;
    public override void OnInteract()
    {
        // Spawn particles
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(leavesClips, 0.3f, true);
        }
    }
}
