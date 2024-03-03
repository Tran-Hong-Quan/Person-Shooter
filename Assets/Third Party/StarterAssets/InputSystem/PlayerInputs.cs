using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : CharacterInputs
{
    [Header("Player Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool isAim;
    public bool isFire;
    public bool changeView;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Events")]
    public UnityEvent<float> onChangeEquipment;

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

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnChangeView(InputValue value)
    {
        ChangeView(value.isPressed);
    }

    public void OnFire(InputValue value)
    {
        FireInput(value.isPressed);
    }

    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }

    private void OnStopFire(InputAction.CallbackContext context)
    {
        StopFireInput();
    }

    private void OnStartFire(InputAction.CallbackContext context)
    {
        StartFireInput();
    }

    private void OnEndMove(InputAction.CallbackContext context)
    {
        StopMoveInput(context.ReadValue<Vector2>());
    }

    private void OnStartMove(InputAction.CallbackContext context)
    {
        StartMoveInput(context.ReadValue<Vector2>());
    }

    private void OnUse(InputValue value)
    {
        Use();
    }

    private void OnDrop(InputValue value)
    {
        Drop();
    }

    private void OnReload(InputValue value)
    {
        Reload();
    }

    private void OnChangeEquipment(InputValue value)
    {
        ChangeEquipment(value.Get<float>());
    }

    private void OnChooseFirstRifle(InputValue value)
    {
        ChooseFirstRifle();
    }
    private void OnChooseSecondRifle(InputValue value)
    {
        ChooseSecondRifle();
    }
    private void OnChoosePiston(InputValue value)
    {
        ChoosePiston();
    }
    private void OnChooseMelee(InputValue value)
    {
        ChooseMelee();
    }
    private void OnChooseBomb(InputValue value)
    {
        ChooseBomb();
    }
    private void OnChooseItem(InputValue value)
    {
        ChooseItem();
    }
    private void OnChooseFist(InputValue value)
    {
        ChooseFist();
    }



#endif


    public void StartMoveInput(Vector2 dir)
    {
        onStartMove?.Invoke(dir);
    }

    public void StopMoveInput(Vector2 dir)
    {
        onStopMove?.Invoke(dir);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
        onMove?.Invoke(move);
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ChangeView(bool newState)
    {
        changeView = newState;
        onChangeView?.Invoke();
    }

    public void AimInput(bool newState)
    {
        isAim = newState;
        onAim?.Invoke();
    }

    public void StartFireInput()
    {
        isFire = true;
        onStartFire?.Invoke();

        loopFireCorotine = LoopFireCorotine();
        StartCoroutine(loopFireCorotine);
    }

    IEnumerator loopFireCorotine;

    private IEnumerator LoopFireCorotine()
    {
        while(true)
        {
            yield return null;
            onFire?.Invoke();
        }
    }

    public void StopFireInput()
    {
        StopCoroutine(loopFireCorotine);

        isFire = false;
        onStopFire?.Invoke();
    }

    public void FireInput(bool newState)
    {
        isFire = newState;
    }

    public void Use()
    {
        onUse?.Invoke();
    }

    public void Drop()
    {
        onDrop?.Invoke();
    }

    public void Reload()
    {
        onReload?.Invoke();
    }

    public void ChangeEquipment(float dir)
    {
        onChangeEquipment?.Invoke(dir);
    }

    public void ChooseFirstRifle()
    {
        onChooseFirstRifle?.Invoke();
    }

    public void ChooseSecondRifle()
    {
        onChooseSecondRifle?.Invoke();
    }

    public void ChoosePiston()
    {
        onChoosePiston?.Invoke();
    }

    public void ChooseMelee()
    {
        onChooseMelee?.Invoke();
    }
    public void ChooseBomb()
    {
        onChooseBomb?.Invoke();
    }
    public void ChooseItem()
    {
        onChooseItem?.Invoke();
    }
    public void ChooseFist()
    {
        onChooseFist?.Invoke();
    }
}
