using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineFollow vcamFollow;
    [SerializeField] float xOffset;
    [SerializeField] float offsetLerpSpeed;
    private float targetXOffset;

    // Update is called once per frame
    void Update()
    {
        targetXOffset = Player.Instance.GetFacingRight() ? xOffset : -xOffset;

        Vector3 currentOffset = vcamFollow.FollowOffset;
        float smoothX = Mathf.Lerp(currentOffset.x, targetXOffset, Time.deltaTime * offsetLerpSpeed);

        vcamFollow.FollowOffset = new Vector3(smoothX, currentOffset.y, currentOffset.z);
    }
}
