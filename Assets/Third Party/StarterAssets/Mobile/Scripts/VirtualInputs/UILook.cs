using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILook : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }

    public CanvasScaler canvasScaler;

    [Header("Settings")]
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [Header("Output")]
    public Event onDragOnScreen;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPost = eventData.position;
    }

    Vector2 lastPost;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastPost;
        lastPost = eventData.position;
        Vector2 outputPosition = ApplyInversionFilter(delta);

        OutputPointerEventValue(outputPosition * GameSetting.sensitivity);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OutputPointerEventValue(Vector2.zero);
    }

    private void OutputPointerEventValue(Vector2 pointerPosition)
    {
        onDragOnScreen.Invoke(pointerPosition);
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if (invertXOutputValue)
        {
            position.x = InvertValue(position.x);
        }

        if (invertYOutputValue)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }

    private void Update()
    {
        
    }
}
