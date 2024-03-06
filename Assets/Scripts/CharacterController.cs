using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Game
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] protected Rig aimRig;
        [SerializeField] protected Rig holdRifleRig;
        [SerializeField] protected Rig aimRifleRig;
        [SerializeField] protected Transform aimObj;

        [Header("Boolen State")]
        [SerializeField] public bool isRotatePlayerWithCam;
        [SerializeField] public bool isFpcam;
        [SerializeField] public bool equipRifle = true;
        [SerializeField] public bool isRifleAiming;
        [SerializeField] public bool isRifleFiring;
        [SerializeField] public bool isRifleReloading;

        [Header("Bone")]
        [SerializeField] protected Transform rightHand;

        [Header("Inputs")]
        [SerializeField] protected CharacterInputs inputs;

        public Transform RightHand => rightHand;
        public Animator Animator => _animator;
        public CharacterInputs Inputs => inputs;

        protected Animator _animator;
        protected new Rigidbody rigidbody;
        public Transform AimObj => aimObj;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(Vector3 motion)
        {
            motion.y = 0;

            Vector3 targetPosition = rigidbody.position + motion;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            bool useVerticalVelocity = true;
            if (useVerticalVelocity) targetVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = targetVelocity;
        }

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

        public virtual void StartRifleFireAnimation()
        {
            if (!equipRifle) return;

            isRotatePlayerWithCam = true;
            isRifleFiring = true;

            _animator.SmoothLayerMask("Rifle Fire", 1);
            _animator.SmoothLayerMask("Rifle Aim", 1);
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

        public virtual void PlayReloadAnimation(System.Action onComplete = null, System.Action onRemoveMag = null,
            System.Action onReload = null, System.Action onAttachMag = null)
        {
            if (isRifleReloading) return;
            isRifleReloading = true;

            onGrabRifleMagazine = onRemoveMag;
            onReloadRifle = onReload;
            onAttachNewRiffleMagazine = onAttachMag;

            int layerMaskId = _animator.GetLayerIndex("Rifle Reload");
            _animator.SmoothLayerMask(layerMaskId, 1);
            _animator.Play("Rifle Reload", layerMaskId, 0);

            var animState = _animator.GetCurrentAnimatorStateInfo(layerMaskId);
            float duration = animState.length / animState.speed;

            aimRifleRig.SmoothRig(0);
            holdRifleRig.SmoothRig(0);

            this.DelayFuction(duration - 0.6f, () =>
            {
                _animator.SmoothLayerMask(layerMaskId, 0, onDone: () =>
                {
                    if (isRifleAiming) aimRifleRig.SmoothRig(1);
                    isRifleReloading = false;
                    holdRifleRig.SmoothRig(1);
                    onComplete?.Invoke();
                });
            });
        }

        System.Action onGrabRifleMagazine;
        System.Action onReloadRifle;
        System.Action onAttachNewRiffleMagazine;
        protected virtual void OnGrabRifleMagazine()
        {
            onGrabRifleMagazine?.Invoke();
        }

        protected virtual void OnReloadRifle()
        {
            onReloadRifle?.Invoke();
        }

        protected virtual void OnAttachNewRiffleMagazine()
        {
            onAttachNewRiffleMagazine.Invoke();
        }

        public virtual void StopHoldRifleAnimation()
        {
            if (isRifleAiming) StopRifleAimAnimation();
            if (isRifleFiring) StopRifleAimAnimation();
            _animator.SmoothLayerMask("Rifle Hold", 0);
            _animator.SmoothLayerMask("Rifle Fire", 0);
            _animator.SmoothLayerMask("Rifle Aim", 0);
            _animator.SmoothLayerMask("Rifle Reload", 0);
            holdRifleRig.SmoothRig(0);
            aimRifleRig.SmoothRig(0);
            equipRifle = false;
        }
    }

}