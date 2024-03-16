using DG.Tweening;
using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UniversalInventorySystem;

namespace Game
{
    public class CharacterController : MonoBehaviour
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
        [SerializeField] public bool isRifleAiming;
        [SerializeField] public bool isRifleFiring;
        [SerializeField] public bool isRifleReloading;

        [SerializeField] public bool equipPistol;
        [SerializeField] public bool isPistolAiming;
        [SerializeField] public bool isPistolFiring;
        [SerializeField] public bool isPistolReloading;

        [Header("Bone")]
        [SerializeField] protected Transform rightHand;

        [Header("Inputs")]
        [SerializeField] protected CharacterInputs inputs;

        public Transform RightHand => rightHand;
        public Animator Animator => _animator;
        public CharacterInputs Inputs => inputs;

        protected Animator _animator;
        protected new Rigidbody rigidbody;
        protected Inventory inventory;
        public Transform AimObj => aimObj;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            inventory = new Inventory(18, true, InventoryController.AllInventoryFlags, true);
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

            isRifleAiming = true;
            isRotatePlayerWithCam = true;
            _animator.SmoothLayerMask("Rifle Aim", 1);
            aimRifleRig.SmoothRig(1);
        }

        public virtual void StopRifleAimAnimation()
        {
            if (!equipRifle) return;

            isRifleAiming = false;

            if (!isRifleFiring)
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
            if (isRifleReloading) return;

            isRotatePlayerWithCam = true;
            isRifleFiring = true;

            _animator.SmoothLayerMask("Rifle Fire", 1);
            _animator.SmoothLayerMask("Rifle Aim", 1);
            _animator.Play("Fire", _animator.GetLayerIndex("Rifle Fire"), 0);
            aimRifleRig.SmoothRig(1);
        }

        public virtual void StopRifleFireAnimation()
        {
            if (!equipRifle) return;

            isRifleFiring = false;

            _animator.SmoothLayerMask("Rifle Fire", 0);

            if (!isRifleAiming)
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
            if (isRifleReloading) return;
            isRifleReloading = true;

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

            reloadCorotine =  this.DelayFuction(duration - 0.6f, () =>
            {
                reloadTween = _animator.SmoothLayerMask(layerMaskId, 0, onDone: () =>
                {
                    if (isRifleAiming) aimRifleRig.SmoothRig(1);
                    isRifleReloading = false;
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
            if (isRifleAiming) StopRifleFireAnimation();
            if (isRifleFiring) StopRifleAimAnimation();
            reloadTween?.Kill();
            if(reloadCorotine != null)
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
        }

        #endregion Rifle

        public virtual void PlayHoldingPistolAnimation()
        {
            equipPistol = true;
        }

        public virtual void StartPistolAimAnimtion()
        {
            if (!equipPistol) return;

            isPistolAiming = true;

            aimPistoleRig.SmoothRig(1);
            _animator.SmoothLayerMask("Pistol Aim", 1);
        }

        public virtual void StopPistolAimAnimation()
        {
            isPistolAiming = false;

            if (!isPistolFiring)
            {
                aimPistoleRig.SmoothRig(0);
                _animator.SmoothLayerMask("Pistol Aim", 0);
            }
        }

        public virtual void StartPistolFiringAnimation()
        {
            if(isPistolReloading) return;
            isPistolFiring = true;

            _animator.SmoothLayerMask("Pistol Aim", 1);
            //_animator.SmoothLayerMask("Pistol Fire", 1);
            aimPistoleRig.SmoothRig(1);
        }

        public virtual void StopPistolFireAnimation()
        {
            isPistolFiring = false;
            //_animator.SmoothLayerMask("Pistol Fire", 0);
            if (!isPistolAiming)
            {
                aimPistoleRig.SmoothRig(0);
                _animator.SmoothLayerMask("Pistol Aim", 0);
            }
        }

        public virtual void PlayPistolReloadAnimtion(NoParamaterDelegate onDone = null)
        {
            if (isPistolReloading) return;
            isPistolReloading = true;

            int layerMaskId = _animator.GetLayerIndex("Pistol Reload");
            _animator.SmoothLayerMask(layerMaskId, 1);
            _animator.Play("Reload", layerMaskId, 0);
            var animState = _animator.GetCurrentAnimatorStateInfo(layerMaskId);
            float duration = animState.length / animState.speed;

            onDone += DoneReload;

            this.DelayFuction(duration - 0.5f, onDone);

            void DoneReload()
            {
                _animator.SmoothLayerMask(layerMaskId, 0);
                isPistolReloading = false;
            }
        }

        public virtual void StopHoldingPistolAnimation()
        {
            equipPistol = false;
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

        protected void OnCollisionEnter(Collision collision)
        {
            
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("InventoryItem"))
            {
                if(other.TryGetComponent(out InventoryItem invItem))
                {
                    invItem.AddItemToInventory(inventory);
                }
            }
        }
    }

}