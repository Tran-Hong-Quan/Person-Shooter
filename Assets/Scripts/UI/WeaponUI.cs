using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public ScaleImageWithTexture icon;

    public TMP_Text bulletText;

    private void Awake()
    {
        if (icon == null) icon = transform.GetChild(0).GetComponent<ScaleImageWithTexture>();
        if (bulletText == null) bulletText = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetIcon(Sprite sprite)
    {
        icon.SetSprite(sprite);
    }

    public void ClearBulletText()
    {
        bulletText.text = "";
    }

    public void Select()
    {
        var imageColor = icon.image.color;
        imageColor.a = 1;
        icon.image.color = imageColor;
    }

    public void Unselect()
    {
        var imageColor = icon.image.color;
        imageColor.a = 0.3f;
        icon.image.color = imageColor;
    }

    public void SetBulletText(int current, int avalable)
    {
        bulletText.text = current.ToString() + "/" + avalable.ToString();
    }
}
