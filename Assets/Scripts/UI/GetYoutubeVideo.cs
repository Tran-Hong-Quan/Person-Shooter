using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using YoutubePlayer;

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
            if (videoUri == "") return;
            Debug.Log("Get Uri success: " + videoUri);

            if (videoUri.Contains("https://www.youtube.com/watch?v="))
            {
                PlayVideo(videoUri);
            }    
            else
            {
                videoPlayer.url = videoUri;
                videoPlayer.Play();
                loadingUI.gameObject.SetActive(false);
            }    
        });
    }

    private async void PlayVideo(string uri)
    {
        await videoPlayer.PlayYoutubeVideoAsync(uri);
        loadingUI.gameObject.SetActive(false);
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
