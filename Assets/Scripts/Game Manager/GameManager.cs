using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameManager>("GameManager"));
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public Transition transition;
}
