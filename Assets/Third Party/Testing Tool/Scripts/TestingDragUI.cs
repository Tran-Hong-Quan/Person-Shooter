using UnityEngine;
using UnityEngine.EventSystems;

public class TestingDragUI : MonoBehaviour, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;

    protected virtual void Start()
    {
        canvas = TestingMenu.instance.canvas;
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
