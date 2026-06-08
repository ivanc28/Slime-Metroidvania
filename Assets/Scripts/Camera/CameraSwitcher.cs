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
        if (collision.gameObject.CompareTag("Player") || (collision.GetComponent<BubbleObj>() != null && Player.Instance.IsInBubble()))
        {
            controller.ChangeCamera(thisVCamera);
            UsingCamera = true;
        }
        Debug.Log($"player enter: {collision.gameObject.CompareTag("Player")}, bubble enter: {collision.GetComponent<BubbleObj>() != null}");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") && !Player.Instance.IsInBubble()) || (collision.GetComponent<BubbleObj>() != null && Player.Instance.IsInBubble()))
        {
            controller.ChangeCamera(mainVCamera);
            UsingCamera = false;
        }
        Debug.Log($"player exit: {collision.gameObject.CompareTag("Player")}, bubble exit: {collision.GetComponent<BubbleObj>() != null}");
    }
    //public void OnPlayerExitBubble()
    //{
    //    Collider2D playerCol = Player.Instance.GetComponent<Collider2D>();
    //    Collider2D thisCol = GetComponent<Collider2D>();

    //    if (!playerCol.IsTouching(thisCol))
    //    {
    //        controller.ChangeCamera(mainVCamera);
    //    }
    //}
    //public void CheckColliderPlayerInBubble(Collider2D bubbleCol, ContactFilter2D filter)
    //{
    //    if (Player.Instance.IsInBubble())
    //    {
    //        Debug.Log("In bubble and checking for camera");
    //        Collider2D thisCol = GetComponent<Collider2D>();
    //        bool isTouching = bubbleCol.IsTouching(thisCol, filter);
    //        if (!isTouching)
    //        {
    //            controller.ChangeCamera(mainVCamera);
    //            Debug.Log($"Not using {name}");
    //        }
    //        else
    //        {
    //            controller.ChangeCamera(thisVCamera);
    //            Debug.Log($"Using {name}");
    //        }
    //    }
    //}
}
