using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] CameraController controller;
    [SerializeField] CinemachineCamera mainVCamera;
    [SerializeField] CinemachineCamera thisVCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.ChangeCamera(thisVCamera);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.ChangeCamera(mainVCamera);
        }
    }
}
