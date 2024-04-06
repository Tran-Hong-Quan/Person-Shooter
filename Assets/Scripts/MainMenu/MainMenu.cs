using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public LoadingUI loadingUI;

    public UIAnimation youtubeVideo;
    public GameObject downloadData;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Download Data", 0) == 0)
        {
            downloadData.SetActive(true);
        }
        else
        {
            youtubeVideo.Show();
        }

    }

    public void PlayGame()
    {
        GameManager.instance.transition.FullScreen(
            () => SceneManager.LoadScene("MainMap"), null);
    }
}
