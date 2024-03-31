using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] UIAnimation gameSettingParent;

    private void Start()
    {
        gameSettingParent.gameObject.SetActive(true);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1);
        float musicValue = PlayerPrefs.GetFloat("SFXVolume", 1);
        SetVolumeSFX(sfxValue);
        SetVolumeMusic(musicValue);
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
}
