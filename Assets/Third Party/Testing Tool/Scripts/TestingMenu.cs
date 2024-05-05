using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingMenu : MonoBehaviour
{
    public static TestingMenu instance;

    public Canvas canvas;
    public bool dontDestroyOnLoad;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
