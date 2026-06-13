using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFX : MonoBehaviour
{
    public bool randomPitch;
    public AudioClip hoverClip;
    public AudioSource buttonSource;
    public Sprite unselectedButton;
    public Sprite selectedButton;
    private List<Button> buttonList = new();
    private void Start()
    { 
        //buttonSource = gameObject.AddComponent<AudioSource>();
        float pitch = buttonSource.pitch;
        buttonSource.pitch = randomPitch ? Random.Range(pitch - 0.1f, pitch + 0.1f) : pitch;
    }
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    buttonSource.PlayOneShot(hoverClip);
    //}
    public void PointerEnter(BaseEventData eventData)
    {
        buttonSource.PlayOneShot(hoverClip);
        PointerEventData pointerData = (PointerEventData)eventData;
        if (pointerData.pointerEnter != null)
        {
            // 2. Use InParent so it works if hovering over child text/icons
            Button clickedObject = pointerData.pointerEnter.GetComponentInParent<Button>();

            if (clickedObject != null)
            {
                if (!buttonList.Contains(clickedObject))
                {
                    buttonList.Add(clickedObject);
                }
                clickedObject.image.sprite = selectedButton;
            }
        }
    }

    public void PointerExit(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        if (pointerData.pointerEnter != null)
        {
            Button exitedObject = pointerData.pointerEnter.GetComponentInParent<Button>();

            if (exitedObject != null)
            {
                exitedObject.image.sprite = unselectedButton;
            }
        }

    }

    public List<Button> GetUIButtons()
    {
        return buttonList;
    }

    public void ResetButtonSprites()
    {
        foreach (Button button in GetUIButtons())
        {
            button.image.sprite = unselectedButton;
        }
    }
}
