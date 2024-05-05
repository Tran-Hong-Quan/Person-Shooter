using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputLook : MonoBehaviour
{
    public Vector2 touchInput;
    public Vector2Int invertInput = Vector2Int.one;
    public PlayerController playerController;

    private void Awake()
    {
        if(invertInput.x > 0) invertInput.x = 1; else invertInput.x = -1;
        if(invertInput.y > 0) invertInput.y = 1; else invertInput.y = -1;
    }

    private void Update()
    {
        if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
        {
            touchInput = Touchscreen.current.touches[0].delta.ReadValue();
            touchInput *= GameSetting.sensitivity;
            touchInput *= invertInput;
        }
        else
        {
            touchInput = Vector2.zero;
        }
        playerController.Inputs.LookInput(touchInput);
    }
}
