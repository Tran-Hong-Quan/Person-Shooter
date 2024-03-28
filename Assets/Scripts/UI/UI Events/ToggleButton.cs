using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleButton : MonoBehaviour, IPointerUpHandler
{
    public bool isPressed;
    public UnityEvent<bool> onToggle;
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed ^= true;
        onToggle?.Invoke(isPressed);
    }
}
