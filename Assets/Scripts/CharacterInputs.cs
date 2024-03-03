using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInputs : MonoBehaviour
{
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
}
