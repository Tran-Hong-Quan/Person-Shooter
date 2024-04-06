using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadData : MonoBehaviour
{
    public UIAnimation youtubeVideo;

    public Slider loadingSlider;
    public TMP_Text loadingValue;
    public Transform loadingIcon;

    void Start()
    {
        GameManager.instance.addressableHelper.LoadPacket("GameDataTag", OnLoadingPacket, OnDoneDownloadingPacket);
    }

    private void OnLoadingPacket(float percentage)
    {
        loadingSlider.value = Mathf.Lerp(0, 0.9f, percentage);
        UpdateLoadingValueText(percentage);
    }

    private void OnDoneDownloadingPacket(bool isSuccess)
    {
        loadingSlider.DOValue(1, 1).OnComplete(() =>
        {
            if (isSuccess)
                PlayerPrefs.SetInt("Download Data", 1);
            UpdateLoadingValueText(1);
        }).OnUpdate(() => UpdateLoadingValueText(loadingSlider.value));

        youtubeVideo.Show();
        gameObject.SetActive(false);
    }

    private void UpdateLoadingValueText(float percentage)
    {
        loadingValue.text = (((int)(percentage * 100 * 100)) / 100f).ToString();
    }

    private void Update()
    {
        loadingIcon.Rotate(Vector3.forward, 30 * Time.deltaTime);
    }
}
