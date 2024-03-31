using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void PlayGame()
    {
        GameManager.instance.transition.FullScreen(
            () => SceneManager.LoadScene("MainMap"), null);
    }
}
