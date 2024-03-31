using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public LoadingUI loadingUI;
    public Gallery gallery;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    public void PlayGame()
    {
        GameManager.instance.transition.FullScreen(
            () => SceneManager.LoadScene("MainMap"), null);
    }
}
