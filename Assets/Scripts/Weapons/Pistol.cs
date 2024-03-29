using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pistol : Gun
{
    protected override void StartFire()
    {
        base.StartFire();
        characterController.StartPistolFiringAnimation();
    }

    protected override void StopFire()
    {
        base.StopFire();
        characterController.StopPistolFireAnimation();
    }

    protected override void Fire()
    {
        base.Fire();
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 50);
    }

    protected override void Reload()
    {
        if (isReloading) return;
        isReloading = true;

        audioSource.clip = reloadAudioClip;
        audioSource.Play();
        isFire = false;

        characterController.PlayPistolReloadAnimtion(onDone: () =>
        {
            currentBullet = magazineBullet;
            isReloading = false;
            onChangeBulletCount?.Invoke(currentBullet);
        });
    }

    protected override void StartAim()
    {
        if (isReloading) return;

        isAiming = true;
        characterController.StartPistolAimAnimtion();
    }

    protected override void StopAim()
    {
        if (isReloading) return;

        isAiming = false;
        characterController.StopPistolAimAnimation();
    }

    protected override void OnDestroy()
    {

    }

    public override void Equip(EquipController equipController)
    {
        if (equipController == null) return;
        if (equipController == this.equipController) return;
        if (!equipController.CanEquipPistol()) return;

        equipController.InitEquipPistol(this, ref equipStatus, ref equipType);


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
            PutToWaist();
        }

        base.Equip(equipController);
    }

    public override void Hold()
    {
        base.Hold();
        characterController.PlayHoldingPistolAnimation();
    }

    public void PutToWaist()
    {
        SetParentEquipTo(equipController.pistolWaist);
    }

    public override void Unequip(EquipController equipController)
    {
        base.Unequip(equipController);
        equipController.InitUnequipPistol(this);
    }

    public override void Stored()
    {
        base.Stored();
        PutToWaist();
        characterController.StopHoldingPistolAnimation();
    }
}
