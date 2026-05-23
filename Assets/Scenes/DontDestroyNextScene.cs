using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyNextScene : MonoBehaviour
{
    public string nextScene;
    void Awake()
    {
        SceneManager.LoadScene(nextScene);
    }

}
