using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip menuSong;
    [Tooltip("0 is Sweet, 1 is Savory, 2 is Sour, 3 is Bitter")]
    public AudioClip[] regionSongs;
    public float fadeMusicTime;
    public float timeBetweenSongs;
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
    public void ChangeSong(GameManager.Region prevRegion, GameManager.Region currRegion)
    {
        if(currRegion == GameManager.Region.None || prevRegion == currRegion)
        {
            return;
        }
        StartCoroutine(TransitionToNewRegionSong(currRegion));
    }
    private IEnumerator TransitionToNewRegionSong(GameManager.Region region)
    {
        if(region == GameManager.Region.None)
        {
            Debug.LogWarning("Something with the music went wrong");
            yield break;
        }
        AudioClip newClip = null;
        if (region == GameManager.Region.Sweet)
        {
            newClip = regionSongs[0];
        }
        else if (region == GameManager.Region.Savory)
        {
            newClip = regionSongs[1];
        }
        else if (region == GameManager.Region.Sour)
        {
            newClip = regionSongs[2];
        }
        else if (region == GameManager.Region.Bitter)
        {
            newClip = regionSongs[3];
        }
        // ensure we never transition to the same music clip
        if(newClip == musicSource.clip)
        {
            yield break;
        }
        float volume = musicSource.volume;
        float timer = fadeMusicTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, volume, timer / fadeMusicTime);
            yield return null;
        }
        yield return new WaitForSeconds(timeBetweenSongs);

        musicSource.clip = newClip;
        musicSource.Play();
        timer = 0;
        while (timer < fadeMusicTime)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, volume, timer / fadeMusicTime);
            yield return null;
        }
        musicSource.volume = volume;

    }

    public void FadeOutAndDestroy()
    {
        StartCoroutine(FadeOutAndDestroyCoroutine());
    }
    private IEnumerator FadeOutAndDestroyCoroutine()
    {
        float volume = musicSource.volume;
        float timer = fadeMusicTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, volume, timer / fadeMusicTime);
            yield return null;
        }
        yield return null;
        musicSource.volume = volume;
        Destroy(gameObject);
    }
}
