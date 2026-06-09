using UnityEngine;

public class ScissorsWall : Interactables
{
    public AudioClip[] leavesClips;
    public GameObject leavesParticle;
    public override void OnInteract()
    {
        // Spawn particles
        if(leavesParticle != null)
        {
            Instantiate(leavesParticle, GetInteractPoint(), leavesParticle.transform.rotation);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(leavesClips, 0.3f, true);
        }
    }
}
