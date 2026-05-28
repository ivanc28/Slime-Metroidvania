using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] CameraController controller;
    [SerializeField] CinemachineCamera mainVCamera;
    [SerializeField] CinemachineCamera thisVCamera;
    public bool UsingCamera { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.ChangeCamera(thisVCamera);
            UsingCamera = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !Player.Instance.IsInBubble())
        {
            controller.ChangeCamera(mainVCamera);
            UsingCamera = false;
        }
    }
    public void OnPlayerExitBubble()
    {
        Collider2D playerCol = Player.Instance.GetComponent<Collider2D>();
        Collider2D thisCol = GetComponent<Collider2D>();

        if (!playerCol.IsTouching(thisCol))
        {
            controller.ChangeCamera(mainVCamera);
        }
    }
}
