using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEquipController : EquipController
{
    public Sprite transparentIcon;
    public WeaponUI firstRifleUI;
    public WeaponUI secondRifleUI;
    public WeaponUI pistolIUI;

    private PlayerVCamRecoilShake camShakeRecoil;

    private void Awake()
    {
        camShakeRecoil = GetComponent<PlayerVCamRecoilShake>();
    }

    public override void InitEquipRifle(IEquiptableItem rifle, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        base.InitEquipRifle(rifle, ref equipStatus, ref equipType);

        if (equipType == EquipType.LeftBack)
        {
            EquipWeaponUISetup(firstRifleUI, rifle, equipStatus);
            InitRecoilEvent(rifle.parent);
        }
        else if (equipType == EquipType.RightBack)
        {
            EquipWeaponUISetup(secondRifleUI, rifle, equipStatus);
            InitRecoilEvent(rifle.parent);
        }
    }

    public override void InitEquipPistol(IEquiptableItem pistol, ref EquipStatus equipStatus, ref EquipType equipType)
    {
        base.InitEquipPistol(pistol, ref equipStatus, ref equipType);

        if (equipType == EquipType.PistolWaist)
        {
            EquipWeaponUISetup(pistolIUI, pistol, equipStatus);
            InitRecoilEvent(pistol.parent);
        }
    }

    void EquipWeaponUISetup(WeaponUI ui, IEquiptableItem equipment, EquipStatus equipStatus)
    {
        ui.SetIcon(equipment.IconSprite);
        var gun = equipment.parent.GetComponent<Gun>();
        ui.SetBulletText(gun.CurrentBullet, 999);
        gun.onChangeBulletCount.AddListener(currentBullet=>UpdateBulletText(currentBullet,ui));

        if (equipStatus == EquipStatus.BeingHeld)
        {
            ui.Select();
        }
        else
        {
            ui.Unselect();
        }
    }

    private void UpdateBulletText(int currentBullet,WeaponUI ui)
    {
        ui.SetBulletText(currentBullet, 999);
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
        if (rifle.EquipType == EquipType.LeftBack)
        {
            firstRifleUI.SetIcon(transparentIcon);
            firstRifleUI.ClearBulletText();
            if (rifle == holdingEquipment)
            {
                firstRifleUI.Unselect();
            }
            firstRifleUI.ClearBulletText();
        }
        else if (rifle.EquipType == EquipType.RightBack)
        {
            secondRifleUI.SetIcon(transparentIcon);
            secondRifleUI.ClearBulletText();
            if (rifle == holdingEquipment)
            {
                secondRifleUI.Unselect();
            }
            secondRifleUI.ClearBulletText();
        }

        DeinitRecoilEvent(rifle.parent);

        base.InitUnequipRifle(rifle);
    }

    public override void InitUnequipPistol(IEquiptableItem pistol)
    {
        pistolIUI.SetIcon(transparentIcon);
        pistolIUI.ClearBulletText();
        //if grabing pistol
        if (pistol == holdingEquipment)
        {
            pistolIUI.Unselect();
        }
        pistolIUI.ClearBulletText();
        DeinitRecoilEvent(pistol.parent);
        base.InitUnequipPistol(pistol);
    }

    private void InitRecoilEvent(Transform gunParent)
    {
        if (!gunParent.TryGetComponent(out Gun gun)) return;

        gun.onFire += camShakeRecoil.Shake;
    }

    private void DeinitRecoilEvent(Transform gunParent)
    {
        if (!gunParent.TryGetComponent(out Gun gun)) return;

        gun.onFire -= camShakeRecoil.Shake;
    }
}
