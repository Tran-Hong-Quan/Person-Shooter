using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UniversalInventorySystem;

namespace Game
{
    public class CharacterController : Game.Entity, IHealth
    {
        [Header("Rig")]
        [SerializeField] protected Rig aimRig;
        [SerializeField] protected Rig holdRifleRig;
        [SerializeField] protected Rig aimRifleRig;
        [SerializeField] protected Rig aimPistoleRig;
        [SerializeField] protected Transform aimObj;

        [Header("Boolen State")]
        [SerializeField] public bool isRotatePlayerWithCam;
        [SerializeField] public bool isFpcam;

        [SerializeField] public bool equipRifle;
        [SerializeField] public bool equipPistol;
        [SerializeField] public bool isAiming;
        [SerializeField] public bool isFiring;
        [SerializeField] public bool isReloading;

        [Header("Bone")]
        [SerializeField] protected Transform rightHand;
        [SerializeField] protected bool isUseRagdoll;

        [Header("Inputs")]
        [SerializeField] protected CharacterInputs inputs;
        [SerializeField] protected EquipController equipController;

        [Header("Fields")]
        public float maxHealth = 100f;
        public float currentHealth;
        public HealthTeamSide heathteamSide;

        [Header("Skill")]
        public FreeFireSkill freeFireSkill;

        public Transform RightHand => rightHand;
        public Animator Animator => _animator;
        public CharacterInputs Inputs => inputs;

        protected Animator _animator;
        protected new Rigidbody rigidbody;
        protected Inventory inventory;
        public Transform AimObj => aimObj;

        public Inventory Inventory => inventory;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public EquipController EquipController => equipController;
        public GameObject GameObject => gameObject;

        public HealthTeamSide HealthTeamSide => heathteamSide;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            inventory = new Inventory(18, true, InventoryController.AllInventoryFlags, true);
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
        }

        public void Move(Vector3 motion)
        {
            Vector3 targetPosition = transform.position + motion;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            targetVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = targetVelocity;
        }
        public void Move(Vector3 dir, float speed)
        {
            dir.y = 0;
            rigidbody.AddForce(dir * speed * 10f, ForceMode.Force);
        }

        #region Rifle

        public virtual void StartRifleAimAnimation()
        {
            if (!equipRifle) return;

            isAiming = true;
            isRotatePlayerWithCam = true;
            _animator.SmoothLayerMask("Rifle Aim", 1);
            aimRifleRig.SmoothRig(1);
        }

        public virtual void StopRifleAimAnimation()
        {
            if (!equipRifle) return;

            isAiming = false;

            if (!isFiring)
            {
                _animator.SmoothLayerMask("Rifle Aim", 0, onDone: () => isRotatePlayerWithCam = false);

                aimRifleRig.SmoothRig(0);
            }
        }

        public virtual Vector3 GetAimPoint()
        {
            return aimObj.transform.position;
        }

        public virtual void StartRifleFireAnimation()
        {
            if (!equipRifle) return;
            if (isReloading) return;

            isRotatePlayerWithCam = true;
            isFiring = true;

            _animator.SmoothLayerMask("Rifle Fire", 1);
            _animator.SmoothLayerMask("Rifle Aim", 1);
            _animator.Play("Fire", _animator.GetLayerIndex("Rifle Fire"), 0);
            aimRifleRig.SmoothRig(1);
        }

        public virtual void StopRifleFireAnimation()
        {
            if (!equipRifle) return;

            isFiring = false;

            _animator.SmoothLayerMask("Rifle Fire", 0);

            if (!isAiming)
            {
                _animator.SmoothLayerMask("Rifle Aim", 0, onDone: () => { isRotatePlayerWithCam = false; });
                aimRifleRig.SmoothRig(0);
            }
        }

        public virtual void PlayHoldRifleAnimation()
        {
            _animator.SmoothLayerMask("Rifle Hold", 1);
            holdRifleRig.SmoothRig(1);
            equipRifle = true;
        }

        public virtual void PlayReloadRifleAnimation(System.Action onComplete = null, System.Action onRemoveMag = null,
            System.Action onReload = null, System.Action onAttachMag = null)
        {
            if (isReloading) return;
            isReloading = true;

            onGunMagazine = onRemoveMag;
            onReloadGun = onReload;
            onAttachNewGunMagazine = onAttachMag;

            int layerMaskId = _animator.GetLayerIndex("Rifle Reload");
            _animator.SmoothLayerMask(layerMaskId, 1);
            _animator.Play("Rifle Reload", layerMaskId, 0);

            var animState = _animator.GetCurrentAnimatorStateInfo(layerMaskId);
            float duration = animState.length / animState.speed;

            aimRifleRig.SmoothRig(0);
            holdRifleRig.SmoothRig(0);

            reloadCorotine = this.DelayFunction(duration - 0.6f, () =>
            {
                reloadTween = _animator.SmoothLayerMask(layerMaskId, 0, onDone: () =>
                {
                    if (isAiming) aimRifleRig.SmoothRig(1);
                    isReloading = false;
                    holdRifleRig.SmoothRig(1);
                    onComplete?.Invoke();
                });
            });
        }

        System.Action onGunMagazine;
        System.Action onReloadGun;
        System.Action onAttachNewGunMagazine;

        IEnumerator reloadCorotine;
        Tweener reloadTween;

        protected virtual void OnGrabRifleMagazine()
        {
            onGunMagazine?.Invoke();
        }

        protected virtual void OnReloadRifle()
        {
            onReloadGun?.Invoke();
        }

        protected virtual void OnAttachNewRiffleMagazine()
        {
            onAttachNewGunMagazine?.Invoke();
        }

        public virtual void StopHoldRifleAnimation()
        {
            StopRifleFireAnimation();
            StopRifleAimAnimation();
            reloadTween?.Kill();
            if (reloadCorotine != null)
            {
                StopCoroutine(reloadCorotine);
                reloadTween = null;
            }
            _animator.SmoothLayerMask("Rifle Hold", 0);
            _animator.SmoothLayerMask("Rifle Fire", 0);
            _animator.SmoothLayerMask("Rifle Aim", 0);
            _animator.SmoothLayerMask("Rifle Reload", 0);
            holdRifleRig.SmoothRig(0);
            aimRifleRig.SmoothRig(0);
            equipRifle = false;
            ResetGunState();

        }

        #endregion Rifle

        #region Pistol
        public virtual void PlayHoldingPistolAnimation()
        {
            equipPistol = true;
        }

        public virtual void StartPistolAimAnimtion()
        {
            if (!equipPistol) return;

            isAiming = true;

            aimPistoleRig.SmoothRig(1);
            _animator.SmoothLayerMask("Pistol Aim", 1);
        }

        public virtual void StopPistolAimAnimation()
        {
            isAiming = false;

            if (!isFiring)
            {
                aimPistoleRig.SmoothRig(0);
                _animator.SmoothLayerMask("Pistol Aim", 0);
            }
        }

        public virtual void StartPistolFiringAnimation()
        {
            if (isReloading) return;
            isFiring = true;

            _animator.SmoothLayerMask("Pistol Aim", 1);
            //_animator.SmoothLayerMask("Pistol Fire", 1);
            aimPistoleRig.SmoothRig(1);
        }

        public virtual void StopPistolFireAnimation()
        {
            isFiring = false;
            //_animator.SmoothLayerMask("Pistol Fire", 0);
            if (!isAiming)
            {
                aimPistoleRig.SmoothRig(0);
                _animator.SmoothLayerMask("Pistol Aim", 0);
            }
        }

        public virtual void PlayPistolReloadAnimtion(NoParamaterDelegate onDone = null)
        {
            if (isReloading) return;
            isReloading = true;

            int layerMaskId = _animator.GetLayerIndex("Pistol Reload");
            _animator.SmoothLayerMask(layerMaskId, 1);
            _animator.Play("Reload", layerMaskId, 0);
            var animState = _animator.GetCurrentAnimatorStateInfo(layerMaskId);
            float duration = animState.length / animState.speed;

            onDone += DoneReload;

            this.DelayFunction(duration - 0.5f, onDone);

            void DoneReload()
            {
                _animator.SmoothLayerMask(layerMaskId, 0);
                isReloading = false;
            }
        }

        public virtual void StopHoldingPistolAnimation()
        {
            equipPistol = false;
            ResetGunState();
            StopPistolFireAnimation();
            StopPistolAimAnimation();
            aimPistoleRig.SmoothRig(0);
            _animator.SmoothLayerMask("Pistol Aim", 0);
            _animator.SmoothLayerMask("Pistol Reload", 0);
            reloadTween?.Kill();
            if (reloadCorotine != null)
            {
                StopCoroutine(reloadCorotine);
                reloadTween = null;
            }
        }

        private void ResetGunState()
        {
            isReloading = false;
            isFiring = false;
            isAiming = false;
        }

        #endregion

        protected void OnCollisionEnter(Collision collision)
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("InventoryItem"))
            //{
            //    if(other.TryGetComponent(out InventoryItem invItem))
            //    {
            //        invItem.AddItemToInventory(inventory);
            //    }
            //}
        }

        public virtual void TakeDamge(float damge, HealthEventHandler caller)
        {
            currentHealth -= damge;
            if (currentHealth < 0)
                Die();
        }

        public virtual void Regeneration(float regeneration, HealthEventHandler caller)
        {
            currentHealth += regeneration;
        }

        protected virtual void Die()
        {
            onDie?.Invoke(this);
            Destroy(gameObject);
        }
    }

}

public interface IHealth
{
    public GameObject GameObject { get; }
    public HealthTeamSide HealthTeamSide { get; }
    public float MaxHealth { get; }
    public float CurrentHealth { get; }
    public void TakeDamge(float damage, HealthEventHandler evt);
    public void Regeneration(float regeneration, HealthEventHandler evt);
}

public class HealthEventHandler
{
    public GameObject caller;
    public HealthTeamSide teamSide;

    public string callerData;

    public HealthEventHandler(GameObject caller, string callerData)
    {
        this.caller = caller;
        this.callerData = callerData;
    }

    public HealthEventHandler(GameObject caller, HealthTeamSide teamSide)
    {
        this.caller = caller;
        this.teamSide = teamSide;
    }

    public HealthEventHandler(GameObject caller)
    {
        this.caller = caller;
    }

    public HealthEventHandler() { }
}

[System.Serializable]
public enum HealthTeamSide
{
    None,
    A,
    B,
    C
}