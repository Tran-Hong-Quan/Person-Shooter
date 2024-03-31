using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationBoard : MonoBehaviour
{
    [SerializeField] UIAnimation UIAnimation;
    [SerializeField] TMP_Text textTMP;
    public void Show(string message)
    {
        gameObject.SetActive(true);
        UIAnimation.Show();
        textTMP.text = message;
    }
}
