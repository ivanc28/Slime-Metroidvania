using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyNextScene : MonoBehaviour
{
    public string nextScene;
    void Start()
    {
        GameManager.Instance.ApplySaveData(SaveSystem.Load());
        string lastSavedRoom = GameManager.Instance.GetLastSavedRoomID();
        if(!string.IsNullOrEmpty(lastSavedRoom))
        {
            GameManager.Instance.IsLoadingSaveData = true;
            SceneManager.LoadScene(lastSavedRoom);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }

    }

}
