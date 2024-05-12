using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour, IEquiptableItem
{
    public Game.CharacterController characterController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float damge = 24f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected int bulletsCount = 120;
    [SerializeField] protected int magazineBullet = 30;
    [SerializeField] protected Vector3 recoilAxis = new Vector3(0, 1, 1);
    [SerializeField] protected float recoilFallBack = 30;
    [SerializeField] protected float recoilForce = 10;
    [SerializeField] protected EquipType equipType;
    [SerializeField] protected Sprite iconSprite;

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
    protected EquipController equipController;
    [SerializeField]
    protected EquipStatus equipStatus;
    protected TransformData fireEffectTransformData;

    protected Rigidbody rb;
    protected Collider col;

    protected int currentBullet;

    public EquipStatus EquipStatus => equipStatus;

    public EquipType EquipType => equipType;

    public Sprite IconSprite => iconSprite;
    public int CurrentBullet => currentBullet;

    public Transform parent => transform;

    public UnityEvent<int> onChangeBulletCount;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    protected virtual void Start()
    {
        fireEffectTransformData = new TransformData(fireEffect.transform);
        fireEffect.gameObject.SetActive(false);
    }

    protected virtual void Aim()
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

    protected float clk = 0;

    protected virtual void Update()
    {
        if (clk > 0) clk -= Time.deltaTime;
        recoilValue = Mathf.Lerp(recoilValue, 0, Time.deltaTime * recoilFallBack);
    }

    protected bool isFire = false;
    protected virtual void StartFire()
    {
        if (isReloading) return;
        isFire = true;
        if (currentBullet <= 0)
        {
            Reload();
            isFire = false;
        }
    }

    protected virtual void StopFire()
    {
        isFire = false;
    }

    protected bool isFireSuccess = false;
    float recoilValue = 0;
    Vector3 hitPos;
    protected virtual void Fire()
    {
        isFireSuccess = false;
        if (!isFire) return;
        if (clk > 0f) return;
        if (currentBullet <= 0) return;
        isFireSuccess = true;

        Vector3 idealPoint = characterController.GetAimPoint();
        Ray ray = new Ray();
        Ray practicalRay = new Ray(firePoint.position, firePoint.forward);
        Ray idealRay = new Ray(firePoint.position, idealPoint - firePoint.position);
        float angle = Vector3.Angle(practicalRay.direction, idealRay.direction);
        //Debug.Log("Angle = " + angle);

        if (angle < 15f || Vector3.SqrMagnitude(idealPoint - firePoint.position) < .1f)
            ray = idealRay;
        else
            ray = practicalRay;

        ray.direction = Quaternion.AngleAxis(recoilValue,
            new Vector3(recoilAxis.x, Random.Range(-recoilAxis.y, recoilAxis.y), recoilAxis.z)) * ray.direction;
        recoilValue += recoilForce;
        if (recoilValue > 60) recoilValue = 60;
        if (characterController is PlayerController)
        {
            var player = characterController as PlayerController;
        }

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var hitEff = SimplePool.Spawn(bulletHitEff);
            hitEff.transform.SetParent(null, true);
            hitEff.transform.position = hit.point;
            hitEff.transform.rotation = Quaternion.LookRotation(hit.normal);
            hitEff.Play();
            this.DelayFunction(hitEff.main.duration, () => SimplePool.Despawn(hitEff.gameObject));
            if (hit.transform.TryGetComponent(out IHealth entity))
            {
                entity.TakeDamge(damge, new HealthEventHandler(gameObject, characterController.heathteamSide));
            }
            //print("Gun fire to " + hit.collider.name);
            hitPos = hit.point;
        }
        else
        {
            hitPos = ray.origin + ray.direction * 10;
        }

        var eff = SimplePool.Spawn(fireEffect);
        fireEffectTransformData.InsertLocalData(eff.transform);
        fireEffectTransformData.SetParent(eff.transform, true);
        eff.Play();
        this.DelayFunction(eff.main.duration, () => SimplePool.Despawn(eff.gameObject));

        audioSource.PlayOneShot(fireAudioClip);

        clk = fireRate;

        if (!characterController.freeFireSkill.isFreeFire)
            currentBullet--;

        onChangeBulletCount?.Invoke(currentBullet);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 50);
    }

    protected bool isReloading;
    protected virtual void Reload()
    {
        onChangeBulletCount?.Invoke(currentBullet);
    }

    protected bool isAiming;

    protected virtual void StartAim()
    {

    }

    protected virtual void StopAim()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    public virtual void Equip(EquipController equipController)
    {
        rb.isKinematic = true;
        col.enabled = false;

        animator = characterController.Animator;
        currentBullet = magazineBullet;
        onChangeBulletCount?.Invoke(currentBullet);
    }

    public virtual void Hold()
    {
        equipStatus = EquipStatus.BeingHeld;
        SetParentEquipTo(equipController.rightHand);
        inputs = characterController.Inputs;
        AddListeners();
        firePoint.forward = equipController.rightHand.up;
    }

    protected virtual void SetParentEquipTo(Transform to)
    {
        transform.SetParent(to, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public virtual void Unequip(EquipController equipController)
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.SetParent(null, true);
        rb.AddForce((characterController.transform.forward + characterController.transform.up).normalized * 500);
        equipStatus = EquipStatus.None;
        equipType = EquipType.None;
        isAiming = false;
        isFire = false;

        this.DelayFunction(1, () => this.equipController = null);
    }

    public virtual void Stored()
    {
        if (equipStatus != EquipStatus.BeingHeld)
        {
            Debug.LogWarning("Store " + name + " failed because it not held");
            return;
        }
        equipStatus = EquipStatus.Stored;
        RemoveListeners();
        characterController.StopHoldRifleAnimation();
        isReloading = false;
        isFire = false;
    }

    public virtual void Use()
    {
        if (equipStatus != EquipStatus.Stored)
        {
            Debug.LogWarning(name + " failed because it not stored");
            return;
        }
        equipStatus = EquipStatus.BeingHeld;
        Hold();
    }

    protected virtual void AddListeners()
    {
        inputs.onAim.AddListener(Aim);
        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);
        inputs.onReload.AddListener(Reload);
    }

    protected virtual void RemoveListeners()
    {
        inputs.onAim.RemoveListener(Aim);
        inputs.onFire.RemoveListener(Fire);
        inputs.onStartFire.RemoveListener(StartFire);
        inputs.onStopFire.RemoveListener(StopFire);
        inputs.onReload.RemoveListener(Reload);
    }
}
