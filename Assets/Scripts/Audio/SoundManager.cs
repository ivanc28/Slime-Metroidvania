using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] int audioSourcePoolSize = 10;
    private List<AudioSource> audioSourcePool = new ();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(source);
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f, bool randomizePitch = false, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        AudioSource source = GetAvailableSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
            source.Play();
        }
    }

    public void PlaySound(AudioClip[] clips, float volume = 1f, bool randomizePitch = false, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clips != null && clips.Length > 0)
        {
            PlaySound(clips[Random.Range(0, clips.Length)], volume, randomizePitch, minPitch, maxPitch);
        }
    }

    public void PlaySoundIfNotPlaying(AudioClip clip, float volume = 1f, bool randomizePitch = false, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying && source.clip == clip)
            {
                return;
            }
        }
        PlaySound(clip, volume, randomizePitch, minPitch, maxPitch);
    }
    public void PlaySoundIfNotPlaying(AudioClip[] clips, float volume = 1f, bool randomizePitch = false, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying && clips.Contains(source.clip))
            {
                return;
            }
        }
        PlaySound(clips, volume, randomizePitch, minPitch, maxPitch);
    }
    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
    }
}