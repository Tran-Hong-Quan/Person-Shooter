using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HongQuan
{
    [RequireComponent(typeof(RawImage))]
    public class ScaleRawImageWithCanvas : MonoBehaviour
    {
        [SerializeField] RectTransform canvas;

        private void Start()
        {
            FitWithCavas();
        }

        public void FitWithCavas()
        {
            if (canvas == null)
                canvas = (RectTransform)transform.root;

            RawImage img = GetComponent<RawImage>();

            img.rectTransform.anchorMax = Vector2.one / 2;
            img.rectTransform.anchorMin = Vector2.one / 2;
            img.rectTransform.anchoredPosition = Vector2.zero;
            img.SetNativeSize();

            float multiplier, t;
            Debug.Log(canvas.sizeDelta);
            multiplier = canvas.sizeDelta.x / img.rectTransform.sizeDelta.x;
            t = canvas.sizeDelta.y / img.rectTransform.sizeDelta.y;
            if (multiplier < t) multiplier = t;
            img.rectTransform.sizeDelta *= multiplier;
        }
    }

}