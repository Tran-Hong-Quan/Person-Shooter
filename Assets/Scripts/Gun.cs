using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;

    PlayerInputs inputs;
    Animator animator;
    Cinemachine.CinemachineVirtualCamera tpCam;
    Cinemachine.CinemachineVirtualCamera tpAimCam;
    Cinemachine.CinemachineVirtualCamera fpCam;
    Rig aimRifleRig;


    private void Start()
    {
        animator = playerController.Animator;
        inputs = playerController.Inputs;
        tpCam = playerController.tpCam;
        fpCam = playerController.fpCam;
        tpAimCam = playerController.tpAimCam;
        aimRifleRig = playerController.aimRifleRig;
        inputs.onAim.AddListener(Aim);
    }

    private void Update()
    {

    }
    bool isAiming;

    private void Aim()
    {
        if (isAiming)
        {
            DisableAim();
        }
        else
        {
            EnableAim();
        }
    }

    private void EnableAim()
    {
        isAiming = true;
        playerController.isAim = true;
        animator.SetLayerWeight(animator.GetLayerIndex("Aim"), 1);
        DOVirtual.Float(aimRifleRig.weight, 1f, 0.5f, value =>
        {
            aimRifleRig.weight = value;
        });
        if (!playerController.isFpcam)
            tpAimCam.Priority = 8;
    }

    private void DisableAim()
    {
        isAiming = false;
        playerController.isAim = false;
        animator.SetLayerWeight(animator.GetLayerIndex("Aim"), 0);
        DOVirtual.Float(aimRifleRig.weight, 0f, 0.5f, value =>
        {
            aimRifleRig.weight = value;
        });

        if (!playerController.isFpcam)
            tpAimCam.Priority = 12;
    }
}
