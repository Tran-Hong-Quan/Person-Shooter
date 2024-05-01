using Cinemachine;
using HongQuan;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rifle : Gun
{
    protected override void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected override void StartFire()
    {
        base.StartFire();
        characterController.StartRifleFireAnimation();
    }

    protected override void StopFire()
    {
        base.StopFire();
        characterController.StopRifleFireAnimation();
    }

    protected override void Fire()
    {
        base.Fire();

        if(isFireSuccess) animator.Play("Fire", animator.GetLayerIndex("Rifle Fire"), 0);
    }

    protected override void Reload()
    {
        if (isReloading) return;
        isReloading = true;
        isFire = false;

        characterController.PlayReloadRifleAnimation(onComplete: () =>
        {
            currentBullet = magazineBullet;
            isReloading = false;
            onChangeBulletCount?.Invoke(currentBullet);
        }, onRemoveMag: () =>
        {
            audioSource.PlayOneShot(removeMagAudioClip);
        }, onReload: () =>
        {
            audioSource.PlayOneShot(reloadAudioClip);
        }, onAttachMag: () =>
        {
            audioSource.PlayOneShot(attachMagAudioClip);
        });
    }

    protected override void StartAim()
    {
        if (isReloading) return;

        isAiming = true;
        characterController.StartRifleAimAnimation();
    }

    protected override void StopAim()
    {
        if (isReloading) return;

        isAiming = false;
        characterController.StopRifleAimAnimation();
    }

    protected override void OnDestroy()
    {

    }

    public override void Equip(EquipController equipController)
    {
        if (equipController == null) return;
        if (equipController == this.equipController) return;
        if (!equipController.CanEquipRifle()) return;

        equipController.InitEquipRifle(this, ref equipStatus, ref equipType);

        if (equipType == EquipType.None)
        {
            return;
        }

        characterController = equipController.characterController;
        this.equipController = equipController;

        if (equipStatus == EquipStatus.BeingHeld)
        {
            Hold();
        }
        else
        {
            PutRifleOnBack();
        }
        
        base.Equip(equipController);
    }

    public override void Hold()
    {
        base.Hold();
        characterController.PlayHoldRifleAnimation();
    }

    public void PutRifleOnBack()
    {
        if (equipType == EquipType.RightBack)
        {
            SetParentEquipTo(equipController.rightBack);
        }
        else if (equipType == EquipType.LeftBack)
        {
            SetParentEquipTo(equipController.leftBack);
        }
    }

    public override void Unequip(EquipController equipController)
    {
        base.Unequip(equipController);

        equipController.InitUnequipRifle(this);
        if (EquipStatus == EquipStatus.BeingHeld)
        {
            RemoveListeners();
            characterController.StopHoldRifleAnimation();
        }
    }

    public override void Stored()
    {
        base.Stored();
        PutRifleOnBack();
        characterController.StopHoldRifleAnimation();
    }
}
