using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : CharacterInputs
{
    public bool canInput = true;

    [Header("Player Input Values")]
    public Vector2 look;
    public bool changeView;
    public bool isOpenInventory;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Player Events")]
    public UnityEvent<float> onChangeEquipment;
    public UnityEvent<bool> onToggleInventory;

#if ENABLE_INPUT_SYSTEM
    public PlayerInput inputs;

    private void Awake()
    {
        inputs = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        var moveAction = inputs.actions.FindAction("Move");
        moveAction.started += OnStartMove;
        moveAction.canceled += OnEndMove;

        var fireAction = inputs.actions.FindAction("Fire");
        fireAction.started += OnStartFire;
        fireAction.canceled += OnStopFire;
    }

    #region Receive Messages

    private void OnMove(InputValue value)
    {
        if (!canInput) return;
        MoveInput(value.Get<Vector2>());
    }

    private void OnLook(InputValue value)
    {
        if (!canInput) return;
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    private void OnJump(InputValue value)
    {
        if (!canInput) return;
        JumpInput(value.isPressed);
    }

    private void OnSprint(InputValue value)
    {
        if (!canInput) return;
        SprintInput(value.isPressed);
    }

    private void OnChangeView(InputValue value)
    {
        if (!canInput) return;
        ChangeView(value.isPressed);
    }

    private void OnFire(InputValue value)
    {
        if (!canInput) return;
        FireInput(value.isPressed);
    }

    private void OnAim(InputValue value)
    {
        if (!canInput) return;
        AimInput(value.isPressed);
    }

    private void OnStopFire(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        StopFireInput();
    }

    private void OnStartFire(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        StartFireInput();
    }

    private void OnEndMove(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        StopMoveInput(context.ReadValue<Vector2>());
    }

    private void OnStartMove(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        StartMoveInput(context.ReadValue<Vector2>());
    }

    private void OnUse(InputValue value)
    {
        if (!canInput) return;
        Use();
    }

    private void OnDrop(InputValue value)
    {
        if (!canInput) return;
        Drop();
    }

    private void OnReload(InputValue value)
    {
        if (!canInput) return;
        Reload();
    }

    private void OnChangeEquipment(InputValue value)
    {
        //ChangeEquipment(value.Get<float>());
    }

    private void OnChooseFirstRifle(InputValue value)
    {
        if (!canInput) return;
        ChooseFirstRifle();
    }
    private void OnChooseSecondRifle(InputValue value)
    {
        if (!canInput) return;
        ChooseSecondRifle();
    }
    private void OnChoosePiston(InputValue value)
    {
        if (!canInput) return;
        ChoosePiston();
    }
    private void OnChooseMelee(InputValue value)
    {
        if (!canInput) return;
        ChooseMelee();
    }
    private void OnChooseBomb(InputValue value)
    {
        if (!canInput) return;
        ChooseBomb();
    }
    private void OnChooseItem(InputValue value)
    {
        if (!canInput) return;
        ChooseItem();
    }
    private void OnChooseFist(InputValue value)
    {
        if (!canInput) return;
        ChooseFist();
    }

    private void OnChooseInventory(InputValue value)
    {
        isOpenInventory = !isOpenInventory;
        ChooseInventory(isOpenInventory);
    }

    #endregion


#endif

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    public void SetCursorState(bool newState)
    {
        cursorLocked = newState;
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ChangeView(bool newState)
    {
        changeView = newState;
        onChangeView?.Invoke();
    }

    public void ChooseInventory(bool value)
    {
        onToggleInventory?.Invoke(value);
    }
}
