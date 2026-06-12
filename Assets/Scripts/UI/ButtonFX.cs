using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFX : MonoBehaviour
{
    public float volume;
    public bool randomPitch;
    public AudioClip hoverClip;
    public AudioSource buttonSource;
    private void Start()
    { 
        //buttonSource = gameObject.AddComponent<AudioSource>();
        buttonSource.volume = volume;
        float pitch = buttonSource.pitch;
        buttonSource.pitch = randomPitch ? Random.Range(pitch - 0.1f, pitch + 0.1f) : pitch;
    }
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    buttonSource.PlayOneShot(hoverClip);
    //}
    public void PlayHoverSFX()
    {
        buttonSource.PlayOneShot(hoverClip);
    }
}
