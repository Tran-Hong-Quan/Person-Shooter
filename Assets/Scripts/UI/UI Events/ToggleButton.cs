using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour, IPointerClickHandler
{
    public bool isOn;
    public Color selectColor = Color.white;
    public UnityEvent<bool> onToggle;

    Image image;
    Color baseColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        baseColor = image.color;
        if (isOn) image.color = selectColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOn ^= true;
        if (isOn) image.color = selectColor; else image.color = baseColor;
        onToggle?.Invoke(isOn);
    }
}
