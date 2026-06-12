using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScene : MonoBehaviour
{
    public Animator sceneTransitionAnim;
    public float fadeTime;
    public string nextScene;
    private bool pressedSpace = false;
    private void Start()
    {
        sceneTransitionAnim.SetTrigger("FadeIn");

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !pressedSpace)
        {
            StartCoroutine(StartGame());
            pressedSpace = true;
        }
    }
    private IEnumerator StartGame()
    {
        sceneTransitionAnim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(nextScene);
    }
}
