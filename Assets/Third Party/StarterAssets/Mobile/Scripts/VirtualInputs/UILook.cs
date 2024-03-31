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
    public static float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [Header("Output")]
    public Event onDragOnScreen;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta / canvasScaler.scaleFactor;

        Vector2 outputPosition = ApplyInversionFilter(delta);

        OutputPointerEventValue(outputPosition * magnitudeMultiplier);
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
}
