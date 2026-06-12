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
        buttonSource.pitch = randomPitch ? Random.Range(0.9f, 1.1f) : 1f;
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
