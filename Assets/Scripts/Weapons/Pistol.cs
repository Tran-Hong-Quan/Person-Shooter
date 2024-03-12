using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IEquiptableItem
{
    public Game.CharacterController characterController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.4f;
    [SerializeField] protected float muzzleVelocity = 0.4f;
    [SerializeField] protected int bulletsCount = 90;
    [SerializeField] protected int magazineBullet = 15;
    [SerializeField] protected EquipType equipType;

    [SerializeField] protected ParticleSystem fireEffect;
    [SerializeField] protected ParticleSystem bulletHitEff;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireAudioClip;
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

    protected TransformData fireEffectTransformData;

    public EquipStatus EquipStatus => equipStatus;

    public EquipType EquipType => equipType;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
        recoil = GetComponent<ProceduralRecoil>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected virtual void Start()
    {
        fireEffectTransformData = new TransformData(fireEffect.transform);
        fireEffect.gameObject.SetActive(false);
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
        characterController.StartPistolFiringAnimation();
        isFire = true;
        if (currentBullet <= 0)
        {
            Reload();
            isFire = false;
        }
    }

    private void StopFire()
    {
        characterController.StopPistolFireAnimation();
        isFire = false;
    }

    private void Fire()
    {
        if (!isFire) return;
        if (clk > 0f) return;

        if (currentBullet <= 0) return;

        Vector3 idealPoint = characterController.AimObj.position;
        Ray ray = new Ray();
        Ray practicalRay = new Ray(firePoint.position, firePoint.forward);
        Ray idealRay = new Ray(firePoint.position, idealPoint - firePoint.position);
        float angle = Vector3.Angle(practicalRay.direction, idealRay.direction);
        //Debug.Log("Angle = " + angle);

        if (angle < 15f || Vector3.SqrMagnitude(idealPoint - firePoint.position) < .1f)
            ray = idealRay;
        else
            ray = practicalRay;


        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var hitEff = SimplePool.Spawn(bulletHitEff);
            hitEff.transform.SetParent(null, true);
            hitEff.transform.position = hit.point;
            hitEff.transform.rotation = Quaternion.LookRotation(hit.normal);
            hitEff.Play();
            this.DelayFuction(hitEff.main.duration, () => SimplePool.Despawn(hitEff.gameObject));
        }

        var eff = SimplePool.Spawn(fireEffect);
        fireEffectTransformData.SetParent(eff.transform);
        fireEffectTransformData.InsertLocalData(eff.transform);
        eff.Play();
        this.DelayFuction(eff.main.duration, () => SimplePool.Despawn(eff.gameObject));

        audioSource.PlayOneShot(fireAudioClip);

        recoil.Aim();

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

        characterController.PlayPistolReloadAnimtion(onDone: () =>
        {
            currentBullet = magazineBullet;
            isReloading = false;
        });
    }

    bool isAiming;

    private void StartAim()
    {
        if (isReloading) return;

        isAiming = true;
        characterController.StartPistolAimAnimtion();
    }

    private void StopAim()
    {
        if (isReloading) return;

        isAiming = false;
        characterController.StopPistolAimAnimation();
    }

    private void OnDestroy()
    {

    }

    public void Equip(EquipController equipController)
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

        rb.isKinematic = true;
        col.enabled = false;

        animator = characterController.Animator;
        currentBullet = magazineBullet;
    }

    public void Hold()
    {
        SetParentEquipTo(equipController.rightHand);
        characterController.PlayHoldingPistolAnimation();
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

    public void PutToWaist()
    {
        SetParentEquipTo(equipController.pistolWaist);
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

        //equipController.InitUnequipRifle(this);

        if (EquipStatus == EquipStatus.BeingHeld)
        {
            RemoveListeners();
            characterController.StopHoldingPistolAnimation();
        }

        //equipController.InitUnequipRifle(this);

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

        characterController.StopHoldingPistolAnimation();
        PutToWaist();
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

        Hold();
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
