using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Object will set active true and false when show and hide")] private bool isSetActive = true;
    protected Vector3 baseScale;
    protected CanvasGroup canvasGroup;

    public UnityEvent<UIAnimation> onShow;
    public UnityEvent<UIAnimation> onHide;

    private void Awake()
    {
        baseScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(TweenCallback onDone)
    {
        if (isSetActive) gameObject.SetActive(true);
        transform.localScale = baseScale * 1.5f;
        transform.DOScale(baseScale, 0.1f).OnComplete(onDone).SetUpdate(true);

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, .1f).OnComplete(() =>
            {
                canvasGroup.interactable = true;
            }).SetUpdate(true);
        }

        onShow?.Invoke(this);
    }

    public void Show()
    {
        Show(null);
    }

    public void Hide()
    {
        Hide(null);
    }

    public void Hide(TweenCallback onDone)
    {
        onDone += () =>
        {
            if(isSetActive) gameObject.SetActive(false);
        };


        transform.localScale = baseScale;
        transform.DOScale(baseScale * 1.5f, 0.1f).OnComplete(onDone).SetUpdate(true);

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0, .1f).SetUpdate(true);
        }

        onHide?.Invoke(this);
    }
}
