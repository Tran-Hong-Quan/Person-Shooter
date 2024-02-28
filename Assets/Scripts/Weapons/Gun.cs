using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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
    }

    float clk = 0;

    private void Update()
    {
        if (clk > 0) clk -= Time.fixedDeltaTime;
        Fire();
    }

    private void Fire()
    {
        bool isFire = inputs.isFire;

        if (isFire)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Firing"), 1);
            playerController.isAim = true;
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Firing"), 0);
            playerController.isAim = false;
        }

        if (!(isFire && clk <= 0)) return;

        Vector3 dir = Vector3.zero;

        Ray ray = mainCam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            dir = hit.point - firePoint.position;
        }
        else
            dir = firePoint.forward;

        fireEffect.Play();
        clk = fireRate;
    }

    bool isAiming;

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
    }

    private void StartAim()
    {
        if (isAiming) return;
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

    private void StopAim()
    {
        if (!isAiming) return;
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
