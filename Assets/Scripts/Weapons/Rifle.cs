using Cinemachine;
using HongQuan;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public Game.CharacterController playerController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected int bulletsCount = 120;
    [SerializeField] protected int magazineBullet = 30;

    [SerializeField] protected ParticleSystem fireEffect;
    [SerializeField] protected ParticleSystem bulletHitEff;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireAudioClip;
    [SerializeField] protected AudioClip removeMagAudioClip;
    [SerializeField] protected AudioClip attachMagAudioClip;
    [SerializeField] protected AudioClip reloadAudioClip;

    protected PlayerInputs inputs;
    protected Camera mainCam;
    protected Animator animator;
    protected ProceduralRecoil recoil;

    protected int currentBullet;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
        recoil = GetComponent<ProceduralRecoil>();
    }

    private void Start()
    {
        inputs = ((PlayerController)playerController).Inputs;

        inputs.onAim.AddListener(Aim);

        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);

        animator = playerController.Animator;

        var recoilTargets = new List<Transform>();
        recoilTargets.Add(((PlayerController)playerController).cameraRoot.GetChild(0));
        foreach (var recoilTarget in ((PlayerController)playerController).camerasRootFollow)
            if (recoilTarget.childCount > 0)
                recoilTargets.Add(recoilTarget.GetChild(0));
        recoil.Init(recoilTargets);

        currentBullet = magazineBullet;

        firePoint.forward = playerController.RightHand.transform.up;
    }

    public virtual void Equip(Game.CharacterController character)
    {
        var charRightHand = character.RightHand;
        transform.SetParent(charRightHand, true);

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    public void Unequiq()
    {

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
    }

    float clk = 0;

    private void Update()
    {
        if (clk > 0) clk -= Time.deltaTime;
    }

    bool isFire = false;
    private void StartFire()
    {
        if (isReloading) return;
        playerController.StartRifleFireAnimation();
        isFire = true;
        if (currentBullet <= 0)
        {
            Reload();
            isFire = false;
        }
    }

    private void StopFire()
    {
        playerController.StopRifleFireAnimation();
        isFire = false;
    }

    private void Fire()
    {
        if (!isFire) return;
        if (clk > 0f) return;

        if (currentBullet <= 0) return;

        Ray ray = new Ray(firePoint.position, firePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var hitEff = SimplePool.Spawn(bulletHitEff);
            hitEff.transform.SetParent(null, true);
            hitEff.transform.position = hit.point;
            hitEff.transform.rotation = Quaternion.LookRotation(hit.normal);
            hitEff.Play();
            this.DelayFuction(hitEff.main.duration, () => SimplePool.Despawn(hitEff.gameObject));
        }

        var eff = SimplePool.Spawn(fireEffect, fireEffect.transform.position, fireEffect.transform.rotation);
        eff.transform.SetParent(fireEffect.transform.parent, true);
        eff.Play();
        this.DelayFuction(eff.main.duration, () => SimplePool.Despawn(eff.gameObject));

        audioSource.PlayOneShot(fireAudioClip);

        recoil.Aim();

        animator.Play("Rifle Fire", animator.GetLayerIndex("Rifle Fire"));

        clk = fireRate;

        currentBullet--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 50);
    }

    bool isReloading;
    private void Reload()
    {
        if (isReloading) return;
        isReloading = true;

        playerController.PlayReloadAnimation(onComplete: () =>
        {
            currentBullet = magazineBullet;
            isReloading = false;
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

    Vector2 recoilValue = Vector2.zero;

    bool isAiming;

    private void StartAim()
    {
        if (isReloading) return;

        isAiming = true;
        playerController.StartRifleAimAnimation();
    }

    private void StopAim()
    {
        if (isReloading) return;

        isAiming = false;
        playerController.StopRifleAimAnimation();
    }

    private void OnDestroy()
    {

    }
}
