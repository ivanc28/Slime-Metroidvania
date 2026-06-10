using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuSong;
    [Tooltip("0 is Sweet, 1 is Savory, 2 is Sour, 3 is Bitter")]
    public AudioClip[] regionSongs;
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
    //private IEnumerator TransitionToNewSong()
    //{
    //    private float volume =
    //}
}
