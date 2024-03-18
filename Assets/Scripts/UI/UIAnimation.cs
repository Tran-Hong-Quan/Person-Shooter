using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    protected Vector3 baseScale;
    protected CanvasGroup canvasGroup;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Show(TweenCallback onDone = null)
    {
        gameObject.SetActive(true);
        transform.localScale = baseScale * 1.5f;
        transform.DOScale(baseScale, 0.1f).OnComplete(onDone);

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, .1f).OnComplete(() =>
            {
                canvasGroup.interactable = true;
            });
        }
    }

    public void Hide(TweenCallback onDone = null)
    {
        onDone += () => gameObject.SetActive(false);
        transform.localScale = baseScale;
        transform.DOScale(baseScale * 1.5f, 0.1f).OnComplete(onDone);

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, .1f);
        }
    }
}
