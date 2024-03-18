using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIToggleButton : MonoBehaviour
{
    public bool value;
    public UnityEvent<bool> toggle;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }

    public void Toggle()
    {
        value = !value;
        toggle?.Invoke(value);
        if(value)
        {
            button.image.color = button.colors.pressedColor;
        }
        else
        {
            button.image.color = button.colors.normalColor;
        }
    }
}
