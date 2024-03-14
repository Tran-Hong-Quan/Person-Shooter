using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EquipController : MonoBehaviour
{
    public Game.CharacterController characterController;

    public Transform rightHand;
    public Transform leftHand;
    public Transform rightBack;
    public Transform leftBack;

    public Transform pistolWaist;

    public IEquiptableItem holdingEquipment;

    public IEquiptableItem rightBackEquipment;
    public IEquiptableItem leftBackEquipment;

    public IEquiptableItem pistonEquipment;
    public IEquiptableItem meleeEquipment;
    public IEquiptableItem[] bombs = new IEquiptableItem[3];

    public IEquiptableItem[] canBeHeldItems = new IEquiptableItem[7];

    private void Start()
    {
        var characterInputs = characterController.Inputs;

        characterInputs.onChooseFirstRifle.AddListener(ChooseFirstRifle);
        characterInputs.onChooseSecondRifle.AddListener(ChooseSecondRifle);
        characterInputs.onChoosePiston.AddListener(ChoosePistol);
        characterInputs.onChooseFist.AddListener(ChooseFist);
        characterInputs.onDrop.AddListener(Unequip);
    }

    public void Equip(IEquiptableItem item)
    {
        item.Equip(this);
    }

    public void Unequip()
    {
        if (holdingEquipment == null) return;
        holdingEquipment.Unequip(this);
        holdingEquipment = null;
    }

    public void StoreHoldingItem()
    {
        if (holdingEquipment == null) return;
        holdingEquipment.Stored();
        holdingEquipment = null;
    }

    public void Use(IEquiptableItem item)
    {
        item.Use();
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        //print("Enter " + collider.gameObject.name);
        if (!collider.CompareTag("Equipment")) return;
        if (!collider.transform.TryGetComponent(out IEquiptableItem item)) return;
        Equip(item);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Equipment")) return;
        if (!collision.transform.TryGetComponent(out IEquiptableItem item)) return;
        Equip(item);
    }

    public void InitEquipRifle(IEquiptableItem rifle, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        equipStatus = EquipStatus.Stored;
        if (leftBackEquipment == null)
        {
            canBeHeldItems[0] = rifle;
            leftBackEquipment = rifle;
            equipType = EquipType.LeftBack;
        }
        else if (rightBackEquipment == null)
        {
            canBeHeldItems[1] = rifle;
            rightBackEquipment = rifle;
            equipType = EquipType.RightBack;
        }
        else
        {
            equipStatus = EquipStatus.None;
            equipType = EquipType.None;
            return;
        }

        if (holdingEquipment == null)
        {
            holdingEquipment = rifle;
            equipStatus = EquipStatus.BeingHeld;
        }
    }

    public void InitUnequipRifle(IEquiptableItem rifle)
    {
        if (rifle == holdingEquipment)
            holdingEquipment = null;
        if (rifle.EquipType == EquipType.LeftBack)
        {
            canBeHeldItems[0] = null;
            leftBackEquipment = null;
        }
        else if (rifle.EquipType == EquipType.RightBack)
        {
            canBeHeldItems[1] = null;
            rightBackEquipment = null;
        }
    }

    public bool CanEquipRifle()
    {
        return leftBackEquipment == null || rightBackEquipment == null;
    }

    public void InitEquipPistol(IEquiptableItem pistol, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        equipStatus = EquipStatus.Stored;
        if (pistonEquipment == null)
        {
            canBeHeldItems[2] = pistol;
            pistonEquipment = pistol;
            equipType = EquipType.PistolWaist;
        }
        else
        {
            equipStatus = EquipStatus.None;
            equipType = EquipType.None;
            return;
        }

        if (holdingEquipment == null)
        {
            holdingEquipment = pistol;
            equipStatus = EquipStatus.BeingHeld;
        }
    }

    public bool CanEquipPistol()
    {
        return pistonEquipment == null;
    }


    //Choose Weopon
    public virtual void ChooseFirstRifle()
    {
        if (leftBackEquipment == null) return;

        if (holdingEquipment == null)
        {
            holdingEquipment = leftBackEquipment;
            leftBackEquipment.Use();
        }
        else if (leftBackEquipment == holdingEquipment)
        {
            holdingEquipment.Stored();
            holdingEquipment = null;
            ChooseFist();
        }
        else if (leftBackEquipment != holdingEquipment)
        {
            holdingEquipment.Stored();
            holdingEquipment = leftBackEquipment;
            holdingEquipment.Use();
        }
    }

    public virtual void ChooseSecondRifle()
    {
        if (rightBackEquipment == null) return;

        if (holdingEquipment == null)
        {
            holdingEquipment = rightBackEquipment;
            rightBackEquipment.Use();
        }
        else if (rightBackEquipment == holdingEquipment)
        {
            holdingEquipment.Stored();
            holdingEquipment = null;
            ChooseFist();
        }
        else if (rightBackEquipment != holdingEquipment)
        {
            holdingEquipment.Stored();
            holdingEquipment = rightBackEquipment;
            holdingEquipment.Use();
        }
    }

    public virtual void ChoosePistol()
    {
        if (pistonEquipment == null) return;

        if (holdingEquipment == null) //Not holding anythings
        {
            holdingEquipment = pistonEquipment;
            holdingEquipment.Use();
        }
        else if (holdingEquipment == pistonEquipment) //Holding pistol
        {
            holdingEquipment.Stored();
            holdingEquipment = null;
            ChooseFist();
        }
        else //Hold something else
        {
            holdingEquipment.Stored();
            holdingEquipment = pistonEquipment;
            holdingEquipment.Use();
        }
    }

    public virtual void ChooseFist()
    {
        if (holdingEquipment == null) return;
        holdingEquipment.Stored();
        holdingEquipment = null;
    }
}

public interface IEquiptableItem
{
    public EquipStatus EquipStatus { get; }
    public EquipType EquipType { get; }
    public void Equip(EquipController equipController);
    public void Unequip(EquipController equipController);
    public void Stored();
    public void Use();
}

[System.Serializable]
public enum EquipStatus
{
    None,
    BeingHeld,
    Stored,
}

[System.Serializable]
public enum EquipType
{
    None,
    RightBack,
    LeftBack,
    PistolWaist,
}
