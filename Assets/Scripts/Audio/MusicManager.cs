using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuSong;
    [Tooltip("0 is Sweet, 1 is Savory, 2 is Sour, 3 is Bitter")]
    public AudioClip[] regionSongs;
    public float fadeMusicTime;
    public static MusicManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ChangeSong()
    {

    }
    private IEnumerator TransitionToNewSong()
    {
        float volume = AudioListener.volume;
        float timer = fadeMusicTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(0, volume, timer / fadeMusicTime);
            yield return null;
        }
    }
}
