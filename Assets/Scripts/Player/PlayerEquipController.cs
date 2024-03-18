using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEquipController : EquipController
{
    [SerializeField] Sprite transparentIcon;
    [SerializeField] WeaponUI firstRifleUI;
    [SerializeField] WeaponUI secondRifleUI;
    [SerializeField] WeaponUI pistolIUI;

    public override void InitEquipRifle(IEquiptableItem rifle, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        base.InitEquipRifle(rifle, ref equipStatus, ref equipType);

        if (equipType == EquipType.LeftBack)
        {
            EquipUISetup(firstRifleUI, rifle, equipStatus);
        }
        else if (equipType == EquipType.RightBack)
        {
            EquipUISetup(secondRifleUI, rifle, equipStatus);
        }
    }

    public override void InitEquipPistol(IEquiptableItem pistol, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        base.InitEquipPistol(pistol, ref equipStatus, ref equipType);

        if (equipType == EquipType.PistolWaist)
        {
            EquipUISetup(pistolIUI, pistol, equipStatus);
        }
    }

    void EquipUISetup(WeaponUI ui, IEquiptableItem equipment, EquipStatus equipStatus)
    {
        ui.SetIcon(equipment.InconSprite);
        if (equipStatus == EquipStatus.BeingHeld)
        {
            ui.Select();
        }
        else
        {
            ui.Unselect();
        }
    }

    public void UnselectAll()
    {
        firstRifleUI.Unselect();
        secondRifleUI.Unselect();
        pistolIUI.Unselect();
    }

    public override void ChooseFirstRifle()
    {
        base.ChooseFirstRifle();
        UnselectAll();
        if (holdingEquipment != null)
        {
            firstRifleUI.Select();
        }
    }

    public override void ChooseSecondRifle()
    {
        base.ChooseSecondRifle();
        UnselectAll();
        if (holdingEquipment != null)
        {
            secondRifleUI.Select();
        }
    }

    public override void ChooseFist()
    {
        base.ChooseFist();
        UnselectAll();
    }

    public override void ChoosePistol()
    {
        base.ChoosePistol();
        UnselectAll();
        if (holdingEquipment != null)
        {
            pistolIUI.Select();
        }
    }

    public override void InitUnequipRifle(IEquiptableItem rifle)
    {
        base.InitUnequipRifle(rifle);

        //Second bit equal 0, it's first weapon 
        if ((unequipData & 1 << 1) == 0)
        {
            firstRifleUI.SetIcon(transparentIcon);
            //First bit equal 1, holding rifle
            if ((unequipData & 1) != 0)
            {
                firstRifleUI.Unselect();
            }
            firstRifleUI.ClearBulletText();
        }
        else //Second bit equip 1, it's second weapon
        {
            secondRifleUI.SetIcon(transparentIcon);
            //First bit equal 1, holding rifle
            if ((unequipData & 1) != 0)
            {
                secondRifleUI.Unselect();
            }
            secondRifleUI.ClearBulletText();
        }
    }

    public override void InitUnequipPistol(IEquiptableItem pistol)
    {
        base.InitUnequipPistol(pistol);

        pistolIUI.SetIcon(transparentIcon);
        if (unequipData == 1)
        {
            pistolIUI.Unselect();
        }
        pistolIUI.ClearBulletText();
    }
}
