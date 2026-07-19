using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public string sceneToLoadOnStart;

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
    }
    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoadOnStart);
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
