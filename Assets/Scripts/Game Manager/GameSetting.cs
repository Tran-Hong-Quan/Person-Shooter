using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public static float sensitivity = 2;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Text sensitivityText;
    [SerializeField] UIAnimation gameSettingParent;

    public static List<Gallery> galleries = new List<Gallery>();

    private void Start()
    {
        gameSettingParent.gameObject.SetActive(true);

        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1);
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1);
        float sensitivityValue = PlayerPrefs.GetFloat("Sensitivity", 1);

        SetVolumeSFX(sfxValue);
        SetVolumeMusic(musicValue);
        SetSentivity(sensitivityValue);

        sfxSlider.value = sfxValue;
        musicSlider.value = musicValue;

        gameSettingParent.gameObject.SetActive(false);
    }

    public void SetVolumeSFX(float volume)
    {
        float volumeInDecibels = Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat("SFXVolume", volumeInDecibels);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetVolumeMusic(float volume)
    {
        float volumeInDecibels = Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat("MusicVolume", volumeInDecibels);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSentivity(float value)
    {
        sensitivity = value;
        value *= 100;
        value = (int)value;
        value /= 100;
        sensitivityText.text = value.ToString();
    }

    public void OutToMainMenu()
    {
        GameManager.instance.transition.FullScreen(() =>
        {
            SceneManager.LoadScene("MainMenu");
            HideSetting();
        }, null);
    }

    public void OpenSetting()
    {
        gameSettingParent.Show();
        Time.timeScale = 0;
    }

    public void HideSetting()
    {
        gameSettingParent.Hide();
        Time.timeScale = 1;
    }

    public void TakeScreenshot()
    {
        var image = SimpleScreenshot.TakeScreenshot(Camera.main);
        image.name = SimpleScreenshot.SaveSprite(image, Gallery.ScreenshotName);
        GameManager.instance.notificationBoard.Show("Took a Screenshot");
        
        foreach(var gallery in galleries)
        {
            gallery.gameObject.SetActive(true);
            gallery.AddImage(image);
            gallery.gameObject.SetActive(false);
        }
    }
}
