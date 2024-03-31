using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingUI : MonoBehaviour
{
    public UnityEvent onInteruptLoading;

    public void OnInterputLoading()
    {
        onInteruptLoading?.Invoke();
    }
}
