using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public string sceneToLoadOnStartGame;
    [SerializeField] Button continueButton;
    [SerializeField] Sprite selectedButtonSprite;
    [Header("Audio Controls")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    // Called by button
    public static float masterValue = 1;
    public static float musicValue = 1;
    public static float sfxValue = 1;
    private void Start()
    {
        masterValue = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicValue = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxValue = PlayerPrefs.GetFloat("soundVolume", 1f);
        masterSlider.value = masterValue;
        musicSlider.value = musicValue;
        sfxSlider.value = sfxValue;
        if(continueButton != null)
        {
            if (!PlayerPrefs.HasKey("jsonData"))
            {
                continueButton.interactable = false;
                continueButton.image.sprite = selectedButtonSprite;
                continueButton.GetComponent<EventTrigger>().enabled = false;
                Color buttonTextColor = continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color;
                ColorBlock continueButtonColors = continueButton.colors;
                buttonTextColor.a = continueButtonColors.disabledColor.a;
                continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = buttonTextColor;
            }
        }
    }
    // Called by button
    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoadOnStartGame);
    }
    // Called by button
    public void StartGameNewSave()
    {
        PlayerPrefs.DeleteKey("jsonData");
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneToLoadOnStartGame);
    }
    // Called by button
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        masterValue = value;
        PlayerPrefs.SetFloat("masterVolume", value);
    }
    public void SetMusicVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        audioMixer.SetFloat("MusicVolume", db);
        musicValue = value;
        PlayerPrefs.SetFloat("musicVolume", value);
    }
    public void SetSFXVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        audioMixer.SetFloat("SoundVolume", db);
        sfxValue = value;
        PlayerPrefs.SetFloat("soundVolume", value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
