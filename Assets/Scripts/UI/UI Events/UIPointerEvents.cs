using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIPointerEvents : MonoBehaviour,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent<PointerEventData> onPointerClick;
    public UnityEvent<PointerEventData> onPointerDown;
    public UnityEvent<PointerEventData> onPointerEnter;
    public UnityEvent<PointerEventData> onPointerExit;
    public UnityEvent<PointerEventData> onPointerUp;

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       onPointerExit?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }
}
