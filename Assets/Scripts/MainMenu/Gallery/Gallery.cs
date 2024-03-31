using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    public static string ScreenshotName = "ScreenshotImage";

    [SerializeField] Button imagePrefab;
    [SerializeField] Transform content;
    [SerializeField] ViewImageInGallery viewImage;

    private void Start()
    {
        var images = SimpleScreenshot.GetAllScreenshot(ScreenshotName);
        AddImage(null);

        foreach (var image in images)
        {
            AddImage(image);
        }
    }

    private void Awake()
    {
        GameSetting.galleries.Add(this);
    }

    private void OnDestroy()
    {
        GameSetting.galleries.Remove(this);
    }

    public void AddImage(Sprite sprite)
    {
        var go = Instantiate(imagePrefab);
        go.gameObject.SetActive(true);
        go.transform.SetParent(content.transform, false);
        if (sprite != null)
            go.image.sprite = sprite;
        go.onClick.AddListener(() => ViewImage(go));
    }

    private void ViewImage(Button button)
    {
        viewImage.ViewImage(button);
    }
}
