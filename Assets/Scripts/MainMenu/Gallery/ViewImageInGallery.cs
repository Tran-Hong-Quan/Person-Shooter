using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewImageInGallery : MonoBehaviour
{
    [SerializeField] UIAnimation UIAnimation;
    [SerializeField] Image viewImage;
    Button currentViewImageButton;

    private void Awake()
    {
        if (UIAnimation == null)
            UIAnimation = GetComponent<UIAnimation>();
    }

    public void ViewImage(Button button)
    {
        UIAnimation.Show();
        currentViewImageButton = button;
        viewImage.sprite = button.image.sprite;
    }

    public void SaveImage()
    {
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) == NativeGallery.Permission.Granted)
        {
            Save();
        }
        else
        {
            NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) != NativeGallery.Permission.Granted)
            {
                GameManager.instance.notificationBoard.Show("Cannot Save Image");
            }
            else
            {
                Save();
            }
        }

        void Save()
        {
            if (currentViewImageButton.image == null) return;
            NativeGallery.SaveImageToGallery(currentViewImageButton.image.sprite.texture,
                "Third Person Survival",
                currentViewImageButton.image.sprite.name, (success, path) =>
                {
                    if (success)
                    {
                        GameManager.instance.notificationBoard.Show("Save Image Successfully");
                    }
                    else
                    {
                        GameManager.instance.notificationBoard.Show("Cannot Save Image");
                    }
                });
        }
    }

    public void DeleteImage()
    {
        if (currentViewImageButton != null)
        {
            Destroy(currentViewImageButton.gameObject);
            SimpleScreenshot.DeleteImage(currentViewImageButton.image.sprite.name);
        }
        UIAnimation.Hide();
    }
}
