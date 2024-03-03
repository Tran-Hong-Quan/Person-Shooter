using Cinemachine;
using HongQuan;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour, IEquiptableItem
{
    public Game.CharacterController characterController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected int bulletsCount = 120;
    [SerializeField] protected int magazineBullet = 30;
    [SerializeField] protected EquipType equipType;

    [SerializeField] protected ParticleSystem fireEffect;
    [SerializeField] protected ParticleSystem bulletHitEff;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireAudioClip;
    [SerializeField] protected AudioClip removeMagAudioClip;
    [SerializeField] protected AudioClip attachMagAudioClip;
    [SerializeField] protected AudioClip reloadAudioClip;

    protected CharacterInputs inputs;
    protected Camera mainCam;
    protected Animator animator;
    protected ProceduralRecoil recoil;
    protected EquipController equipController;
    [SerializeField]
    protected EquipStatus equipStatus;

    protected Rigidbody rb;
    protected Collider col;

    protected int currentBullet;

    public EquipStatus EquipStatus => equipStatus;

    public EquipType EquipType => equipType;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
        recoil = GetComponent<ProceduralRecoil>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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
        characterController.StartRifleFireAnimation();
        isFire = true;
        if (currentBullet <= 0)
        {
            Reload();
            isFire = false;
        }
    }

    private void StopFire()
    {
        characterController.StopRifleFireAnimation();
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

        characterController.PlayReloadAnimation(onComplete: () =>
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
        characterController.StartRifleAimAnimation();
    }

    private void StopAim()
    {
        if (isReloading) return;

        isAiming = false;
        characterController.StopRifleAimAnimation();
    }

    private void OnDestroy()
    {

    }

    public void Equip(EquipController equipController)
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
            HoldRifle();
        }
        else
        {
            PutRifleOnBack();
        }

        rb.isKinematic = true;
        col.enabled = false;

        animator = characterController.Animator;
        currentBullet = magazineBullet;
    }

    public void HoldRifle()
    {
        SetParentEquipTo(equipController.rightHand);
        characterController.PlayHoldRifleAnimation();
        inputs = characterController.Inputs;
        AddListeners();
        firePoint.forward = equipController.rightHand.up;
        if (equipController is PlayerEquipController)
        {
            var recoilTargets = new List<Transform>();
            recoilTargets.Add(((PlayerController)characterController).cameraRoot.GetChild(0));
            foreach (var recoilTarget in ((PlayerController)characterController).camerasRootFollow)
                if (recoilTarget.childCount > 0)
                    recoilTargets.Add(recoilTarget.GetChild(0));
            recoil.Init(recoilTargets);
        }
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

    private void SetParentEquipTo(Transform to)
    {
        transform.SetParent(to, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void Unequip(EquipController equipController)
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.SetParent(null, true);
        rb.AddForce((characterController.transform.forward + characterController.transform.up).normalized * 500);

        equipController.InitUnequipRifle(this);

        if (EquipStatus == EquipStatus.BeingHeld)
        {
            RemoveListeners();
            characterController.StopHoldRifleAnimation();
        }

        equipController.InitUnequipRifle(this);

        recoil.ClearTargets();
        equipStatus = EquipStatus.None;
        equipType = EquipType.None;
        isAiming = false;
        isFire = false;

        this.DelayFuction(1, () => this.equipController = null);
    }

    public void Stored()
    {
        if (equipStatus != EquipStatus.BeingHeld)
        {
            Debug.LogWarning("Store " + name + " failed because it not held");
            return;
        }

        characterController.StopHoldRifleAnimation();
        PutRifleOnBack();
        equipStatus = EquipStatus.Stored;
        recoil.ClearTargets();
        RemoveListeners();

        if (characterController is PlayerController)
        {
            characterController.StopHoldRifleAnimation();
        }
    }

    public void Use()
    {
        if (equipStatus != EquipStatus.Stored)
        {
            Debug.LogWarning(name + " failed because it not stored");
            return;
        }

        equipStatus = EquipStatus.BeingHeld;

        HoldRifle();
    }

    private void AddListeners()
    {
        inputs.onAim.AddListener(Aim);
        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);
        inputs.onReload.AddListener(Reload);
    }

    private void RemoveListeners()
    {
        inputs.onAim.RemoveListener(Aim);
        inputs.onFire.RemoveListener(Fire);
        inputs.onStartFire.RemoveListener(StartFire);
        inputs.onStopFire.RemoveListener(StopFire);
        inputs.onReload.RemoveListener(Reload);
    }

}
