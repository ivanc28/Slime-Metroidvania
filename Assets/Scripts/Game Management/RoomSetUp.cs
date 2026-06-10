using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomSetUp : MonoBehaviour
{
    [SerializeField] SpawnPoint[] spawnPoints;
    [SerializeField] GameManager.Region region;
    [Tooltip("How much vertical speed the player must get when transitioning up")]
    [SerializeField] float minJumpSpeed;
    [Tooltip("How much horizontal speed the player has when moving out of an up transition")]
    [SerializeField] float horizontalSpeedOutOfUp;
    [SerializeField] float transitionTime;
    string roomID;
    private void Awake()
    {
        roomID = SceneManager.GetActiveScene().name;
        GameManager.Instance.AddRoom(roomID);
        GameManager.Instance.SetCurrRoomID(roomID);
    }
    private void Start()
    {
        CinemachineCamera[] cameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach(CinemachineCamera cam in cameras)
        {
            cam.Follow = Player.Instance.transform;
        }
        PlayerTalking.SetNPCsInRoom();
        Player.Instance.EnableToolSelectionCanvas(false);
        string spawnPointID = GameManager.Instance.GetNextSpawnPointID();
        UIManager.Instance.FadeOut();
        if (spawnPointID == null || spawnPointID == string.Empty)
        {
            // Initial call when there is no spawnPointID yet (when GameManager first loaded)
            return;
        }
        bool foundScene = false;
        foreach(SpawnPoint point in spawnPoints)
        {
            if(point.spawnPointID == spawnPointID)
            {
                foundScene = true;
                Player.Instance.transform.position = point.transform.position;
                Player.Instance.EnableMovement(false);
                Player.Instance.SetGravityToFalling();
                Player.Instance.anim.Play("idle", 0, 0);
                Player.Instance.ResetFlipTimer();
                StartCoroutine(MoveOutOfTransition(point, transitionTime));
            }
        }
        if (!foundScene)
        {
            Debug.LogWarning($"Failed to find spawnPointID labeled {spawnPointID}");
        }
        Player.Instance.SetGravityToFalling();
        Player.Instance.DetachHook();
        Player.Instance.ClearTrail();

        // Show region animation if new region
        GameManager.Region prevRegion = GameManager.Instance.GetCurrRegion();
        GameManager.Instance.SetCurrRegion(region);
        if (prevRegion != region)
        {
            UIManager.Instance.DisplayRegionTitle(region);
        }
        if(MusicManager.Instance != null)
        {
            MusicManager.Instance.ChangeSong(prevRegion, region);
        }

    }    

    private IEnumerator MoveOutOfTransition(SpawnPoint spawnPoint, float transitionTime)
    {
        Player.Instance.InSceneTransition = true;
        float timer = 0;
        while (timer < transitionTime)
        {
            if (spawnPoint.spawnDirection == SpawnPoint.Direction.Up)
            {
                float moveYSpeed = Player.Instance.rb.linearVelocityY > minJumpSpeed ? Player.Instance.rb.linearVelocityY : minJumpSpeed;
                //Player.Instance.SetGravityToRising();
                if (spawnPoint.upRight)
                {
                    Player.Instance.rb.linearVelocity = new Vector2(horizontalSpeedOutOfUp, moveYSpeed);
                }
                else
                {
                    Player.Instance.rb.linearVelocity = new Vector2(-horizontalSpeedOutOfUp, moveYSpeed);
                }
                yield return new WaitForSeconds(transitionTime);
                Player.Instance.InSceneTransition = false;
                Player.Instance.EnableMovement(true);
                yield break;
            }
            else if(spawnPoint.spawnDirection == SpawnPoint.Direction.Right)
            {
                float xSpeed = Mathf.Abs(Player.Instance.rb.linearVelocityX);
                //Debug.Log($"xspeed was {xSpeed}, and we must be at least {Player.Instance.data.baseMoveSpeed}");
                if (xSpeed < Player.Instance.data.baseMoveSpeed)
                {
                    Player.Instance.rb.linearVelocityX = Player.Instance.data.baseMoveSpeed;

                }
            }
            else if(spawnPoint.spawnDirection == SpawnPoint.Direction.Left)
            {
                float xSpeed = Mathf.Abs(Player.Instance.rb.linearVelocityX);
                //Debug.Log($"xspeed was {xSpeed}, and we must be at least {Player.Instance.data.baseMoveSpeed}");
                if (xSpeed < Player.Instance.data.baseMoveSpeed)
                {
                    Player.Instance.rb.linearVelocityX = -Player.Instance.data.baseMoveSpeed;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
        Player.Instance.EnableMovement(true);
        Player.Instance.InSceneTransition = false;
    }
}
