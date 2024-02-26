using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool changeView;
    public bool isAim;
    public bool isFire;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Events")]
    public UnityEvent onChangeView;
    public UnityEvent onAim;
    public UnityEvent onFire;

#if ENABLE_INPUT_SYSTEM
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
        Fire(value.isPressed);
    }

    public void OnAim(InputValue value)
    {
        Aim(value.isPressed);
    }

#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
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

    private void ChangeView(bool newState)
    {
        changeView = newState;
        onChangeView?.Invoke();
    }

    private void Aim(bool newState)
    {
        isAim = newState;
        onAim?.Invoke();
    }

    private void Fire(bool newState)
    {
        isFire = newState;
        onFire?.Invoke();
    }
}
