using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected ParticleSystem fireEffect;

    protected PlayerInputs inputs;
    protected Animator animator;
    protected Cinemachine.CinemachineVirtualCamera tpCam;
    protected Cinemachine.CinemachineVirtualCamera tpAimCam;
    protected Cinemachine.CinemachineVirtualCamera fpCam;
    protected Rig aimRifleRig;
    protected Camera mainCam;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        animator = playerController.Animator;
        inputs = playerController.Inputs;
        tpCam = playerController.tpCam;
        fpCam = playerController.fpCam;
        tpAimCam = playerController.tpAimCam;
        aimRifleRig = playerController.aimRifleRig;

        inputs.onAim.AddListener(Aim);

        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);
    }

    private void Aim()
    {
        if (isAiming)
        {
            StopAim();
        }
        else
        {
            StartAim();
        }
        inputs.isAim = isAiming;
    }

    float clk = 0;

    private void Update()
    {
        if (clk > 0) clk -= Time.fixedDeltaTime;
    }

    private void StartFire()
    {
        playerController.isAim = true;
        isFiring = true;

        DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Fire")), 1f, 0.5f, value =>
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Rifle Fire"), value);
        });

        if (!isAiming)
            DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Aim")), 1f, 0.5f, value =>
            {
                animator.SetLayerWeight(animator.GetLayerIndex("Rifle Aim"), value);
            });

        DOVirtual.Float(aimRifleRig.weight, 1f, 0.5f, value =>
        {
            aimRifleRig.weight = value;
        });

        Debug.Log("Start Fire");
    }

    private void StopFire()
    {
        DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Fire")), 0f, 0.5f, value =>
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Rifle Fire"), value);
        });

        if (!isAiming)
        {
            isFiring = false;
            DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Aim")), 0f, 0.5f, value =>
            {
                animator.SetLayerWeight(animator.GetLayerIndex("Rifle Aim"), value);
            });

            DOVirtual.Float(aimRifleRig.weight, 0f, 0.5f, value =>
            {
                aimRifleRig.weight = value;
            }).OnComplete(() => playerController.isAim = false);
        }

        Debug.Log("Stop Fire");
    }

    bool isFiring;

    private void Fire()
    {
        if (clk > 0f) return;

        Ray ray = mainCam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - firePoint.position;
        }


        fireEffect.Play();
        clk = fireRate;
    }

    bool isAiming;

    private void StartAim()
    {
        isAiming = true;
        playerController.isAim = true;

        DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Aim")), 1f, 0.5f, value =>
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Rifle Aim"), value);
        });

        if (!isFiring)
            DOVirtual.Float(aimRifleRig.weight, 1f, 0.5f, value =>
            {
                aimRifleRig.weight = value;
            });
        if (!playerController.isFpcam)
            tpAimCam.Priority = 12;
    }

    private void StopAim()
    {
        isAiming = false;

        DOVirtual.Float(animator.GetLayerWeight(animator.GetLayerIndex("Rifle Aim")), 0f, 0.5f, value =>
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Rifle Aim"), value);
        });

        if (!isFiring)
        {
            DOVirtual.Float(aimRifleRig.weight, 0f, 0.5f, value =>
            {
                aimRifleRig.weight = value;
            }).OnComplete(() => playerController.isAim = false);
        }

        if (!playerController.isFpcam)
            tpAimCam.Priority = 8;
    }

    private void OnDestroy()
    {

    }
}
