using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyNextScene : MonoBehaviour
{
    public string nextScene;
    void Start()
    {
        GameManager.Instance.ApplySaveData(SaveSystem.Load());
        if(!string.IsNullOrEmpty(GameManager.Instance.GetLastSavedRoomID()))
        {
            SceneManager.LoadScene(GameManager.Instance.GetLastSavedRoomID());
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }

    }

}
