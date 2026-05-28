using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineFollow vcamFollow;
    [SerializeField] float xOffset;
    [SerializeField] float offsetLerpSpeed;

    private float targetXOffset;
    private CinemachineCamera[] sceneCameras;
    private void Start()
    {
        sceneCameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
    }
    // Update is called once per frame
    void Update()
    {
        targetXOffset = Player.Instance.GetFacingRight() ? xOffset : -xOffset;

        Vector3 currentOffset = vcamFollow.FollowOffset;
        float smoothX = Mathf.Lerp(currentOffset.x, targetXOffset, Time.deltaTime * offsetLerpSpeed);

        vcamFollow.FollowOffset = new Vector3(smoothX, currentOffset.y, currentOffset.z);
    }
    private CinemachineCamera GetCurrentCamera(CinemachineCamera[] cameras) 
    {
        CinemachineCamera ret = null;
        int highestPriority = -999;
        foreach (CinemachineCamera camera in cameras)
        {
            if(camera.Priority > highestPriority)
            {
                highestPriority = camera.Priority;
                ret = camera;
            }
        }
        return ret;
    }
    public void ChangeCamera(CinemachineCamera newCamera)
    {
        // swap priorities
        CinemachineCamera currCamera = GetCurrentCamera(sceneCameras);
        (currCamera.Priority, newCamera.Priority) = (newCamera.Priority, currCamera.Priority);
        vcamFollow = newCamera.GetComponent<CinemachineFollow>();
    }
    
}
