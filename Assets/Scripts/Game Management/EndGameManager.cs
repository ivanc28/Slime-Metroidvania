using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public static bool HAS_SWEET_TROPHY, HAS_SAVORY_TROPHY, HAS_SOUR_TROPHY, HAS_BITTER_TROPHY;
    [Tooltip("0 is Sweet trophy, 1 is Savory, 2 is Sour, 3 is Bitter")]
    public GameObject[] trophyObjects;
    public Animator screenTransitionAnim;
    public float fadeTime;
    public AudioSource endMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject obj in trophyObjects)
        {
            obj.SetActive(false);
        }
        if (HAS_SWEET_TROPHY)
        {
            trophyObjects[0].SetActive(true);
        }
        if (HAS_SAVORY_TROPHY)
        {
            trophyObjects[1].SetActive(true);
        }
        if (HAS_SOUR_TROPHY)
        {
            trophyObjects[2].SetActive(true);
        }
        if (HAS_BITTER_TROPHY)
        {
            trophyObjects[3].SetActive(true);
        }
        ResetTrophiesClaimed();
        StartCoroutine(WaitToFadeIn());
    }
    private IEnumerator WaitToFadeIn()
    {
        yield return null;
        screenTransitionAnim.SetTrigger("FadeIn");
    }

    public static void ResetTrophiesClaimed()
    {
        HAS_SWEET_TROPHY = false;
        HAS_SAVORY_TROPHY= false;
        HAS_SOUR_TROPHY = false;
        HAS_BITTER_TROPHY = false;
    }

    // called by button
    public void ReturnToMenu()
    {
        Debug.Log("pressed button");
        StartCoroutine(FadeOutAndReturnToMenu());
    }
    private IEnumerator FadeOutAndReturnToMenu()
    {
        screenTransitionAnim.SetTrigger("FadeOut");
        float volume = AudioListener.volume;
        float timer = fadeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(0, volume, timer / fadeTime);
            yield return null;
        }
        yield return null;
        endMusic.Stop();
        AudioListener.volume = volume;
        ResetTrophiesClaimed();
        SceneManager.LoadScene("MainMenu");
    }
}
