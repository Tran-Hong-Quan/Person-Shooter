using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInputs : MonoBehaviour
{
    [Header("Input value")]
    public Vector2 move;
    public bool jump;
    public bool sprint;
    public bool isAim;
    public bool isFire;


    [Header("Events")]
    public UnityEvent onChangeView;

    public UnityEvent onAim;

    public UnityEvent onStartFire;
    public UnityEvent onFire;
    public UnityEvent onStopFire;

    public UnityEvent<Vector2> onStartMove;
    public UnityEvent<Vector2> onMove;
    public UnityEvent<Vector2> onStopMove;

    public UnityEvent onDrop;
    public UnityEvent onUse;
    public UnityEvent onReload;

    //Equip Item
    public UnityEvent onChooseFirstRifle;
    public UnityEvent onChooseSecondRifle;
    public UnityEvent onChoosePiston;
    public UnityEvent onChooseMelee;
    public UnityEvent onChooseBomb;
    public UnityEvent onChooseItem;
    public UnityEvent onChooseFist;

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

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
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
        while (true)
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
