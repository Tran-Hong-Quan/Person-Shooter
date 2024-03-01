using Cinemachine;
using HongQuan;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected float shakeCamAmplitude = 2;
    [SerializeField] protected int bulletsCount = 120;
    [SerializeField] protected int magazineBullet = 30;

    [SerializeField] protected ParticleSystem fireEffect;
    [SerializeField] protected ParticleSystem bulletHitEff;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireAudioClip;

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
        inputs = playerController.Inputs;

        inputs.onAim.AddListener(Aim);

        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);

        animator = playerController.Animator;

        var recoilTargets = new List<Transform>();
        recoilTargets.Add(playerController.CinemachineCameraTarget.GetChild(0));
        foreach (var recoilTarget in playerController.camTargets)
            if (recoilTarget.childCount > 0)
                recoilTargets.Add(recoilTarget.GetChild(0));
        recoil.Init(recoilTargets);

        currentBullet = magazineBullet;
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

    private void StartFire()
    {
        if (isReloading) return;
        playerController.StartRifleFireAnimation();
        if (currentBullet <= 0) Reload();
    }

    private void StopFire()
    {
        playerController.StopRifleFireAnimation();
    }

    private void Fire()
    {
        if (clk > 0f) return;

        if (currentBullet <= 0) return;

        Ray ray = mainCam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - firePoint.position;

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

    bool isReloading;
    private void Reload()
    {
        if(isReloading) return;
        isReloading = true;

        playerController.PlayReloadAnimation(() =>
        {
            currentBullet = magazineBullet;
            isReloading = false;
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
