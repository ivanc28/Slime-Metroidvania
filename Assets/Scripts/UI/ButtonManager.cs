using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public string sceneToLoadOnStart;
    // Called by button
    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoadOnStart);
    }
    // Called by button
    public void QuitGame()
    {
        Application.Quit();
    }
}
