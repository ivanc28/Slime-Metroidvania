using UnityEngine;

public class WindmillInteractable : Interactables
{
    public float spinForce;
    public float maxRotationSpeed;
    public float minSFXInterval;
    public float maxSFXInterval;
    public float rotationSpeedSFXThreshold;
    public float sfxDistanceThreshold;
    public AudioClip windmillClip;
    [SerializeField] Rigidbody2D rb;

    private float sfxTimer = 0f;

    public override void MakeUpdate()
    {
        base.MakeUpdate();
        if (rb.angularVelocity > maxRotationSpeed)
        {
            rb.angularVelocity = maxRotationSpeed;
        }

        if (rb.angularVelocity > rotationSpeedSFXThreshold && Vector2.Distance(transform.position, Player.Instance.transform.position) <= sfxDistanceThreshold)
        {
            sfxTimer -= Time.deltaTime;
            if (sfxTimer <= 0f)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(windmillClip, 1, true);
                }
                // play faster when spinning faster
                sfxTimer = Mathf.Lerp(maxSFXInterval, minSFXInterval, rb.angularVelocity / maxRotationSpeed);
            }
        }
    }
    public override void OnInteract()
    {
        rb.AddTorque(spinForce);
        sfxTimer = minSFXInterval;
    }
}
