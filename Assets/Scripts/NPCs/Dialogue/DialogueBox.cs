using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public TextMeshProUGUI dialogueText;
    public LayoutElement textLayout;
    public float[] preferredWidths;

    public void Initalize(string text)
    {
        dialogueCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        dialogueText.text = text;
        if(text.Length <= 4)
        {
            textLayout.preferredWidth = preferredWidths[0] * GetFactorByFontSize();
        }
        else if (text.Length <= 8)
        {
            textLayout.preferredWidth = preferredWidths[1] * GetFactorByFontSize();
        }
        else if (text.Length <= 12)
        {
            textLayout.preferredWidth = preferredWidths[2] * GetFactorByFontSize();
        }
        else
        {
            textLayout.preferredWidth = preferredWidths[3] * GetFactorByFontSize();
        }
    }

    private float GetFactorByFontSize()
    {
        return dialogueText.fontSize / 9;
    }
}
