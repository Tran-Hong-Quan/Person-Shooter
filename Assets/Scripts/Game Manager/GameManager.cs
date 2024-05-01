using QuanUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        Instantiate(Resources.Load<GameManager>("GameManager"));
    }

    public Transition transition;
    public NotificationBoard notificationBoard;
    public AchivementManager achivementManager;
    public AddressableHelper addressableHelper;

    public int level;
    public int score;
    public string[] levelScenes;

    public void NextLevel()
    {
        level++;
        transition.FullScreen(() =>
        {
            SceneManager.LoadScene(levelScenes[Random.Range(0,levelScenes.Length)]);
            Time.timeScale = 1;
        }, null);
    }
}
