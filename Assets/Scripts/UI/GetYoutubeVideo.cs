using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class GetYoutubeVideo : MonoBehaviour
{
    public string uri = "https://tempquan.000webhostapp.com/GetYoutubeLink.php";
    public VideoPlayer videoPlayer;
    public LoadingUI loadingUI;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 1.0f;
        loadingUI.gameObject.SetActive(true);
        loadingUI.onInteruptLoading.AddListener(StopLoadVideo);

        GetYoutubeLink((videoUri) =>
        {
            loadingUI.gameObject.SetActive(false);
            if (videoUri == "") return;
            videoPlayer.url = videoUri;
            videoPlayer.Play();
        });
    }

    private void StopLoadVideo()
    {
        loadingUI.onInteruptLoading.RemoveListener(StopLoadVideo);
        gameObject.SetActive(false);
    }

    private void GetYoutubeLink(System.Action<string> onDone)
    {
        StartCoroutine(GetText(onDone));
    }

    IEnumerator GetText(System.Action<string> onDone)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            onDone?.Invoke("");
        }
        else
        {
            onDone?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }
}
