using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScaleImageWithTexture : MonoBehaviour
{
    public Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        image.SetNativeSize();

        float textureWidth = image.sprite.texture.width;
        float textureHeight = image.sprite.texture.height;

        RectTransform parentRectTransform = transform.parent as RectTransform;
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        float widthRatio = textureWidth / parentWidth;
        float heightRatio = textureHeight / parentHeight;

        float scaleFactor = Mathf.Max(widthRatio, heightRatio);

        float newWidth = textureWidth / scaleFactor;
        float newHeight = textureHeight / scaleFactor;

        image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
