using Cinemachine;
using DG.Tweening;
using HongQuan;
using UnityEngine;
using UnityEngine.Animations.Rigging;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerController : Game.CharacterController
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    public float RotateSpeed = 15.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public Transform footAudioPos;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -9.81f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform cameraRoot;
    public Transform[] camerasRootFollow;
    public float aimDistance = 10f;
    public CinemachineVirtualCamera fpCam;
    public CinemachineVirtualCamera tpCam;
    public CinemachineVirtualCamera tpAimCam;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    public bool UseMouse = true;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private PlayerInputs _input;
    private Transform _mainCameraTransform;
    private Camera _mainCamera;
    private Transform cineCamTarget;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse" && UseMouse;
#else
				return false;
#endif
        }
    }

    public new PlayerInputs Inputs { get { return _input; } }


    protected override void Awake()
    {
        base.Awake();

        // get a reference to our main camera
        if (_mainCameraTransform == null)
        {
            _mainCamera = Camera.main;
            _mainCameraTransform = _mainCamera.transform;
        }
        _input = GetComponent<PlayerInputs>();
        _hasAnimator = TryGetComponent(out _animator);
        cineCamTarget = cameraRoot.GetChild(0);
    }

    private void Start()
    {
        _cinemachineTargetYaw = cameraRoot.rotation.eulerAngles.y;

        InitCameraView();

#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        AssignAnimationIDs();

    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        Jump();
        GroundedCheck();
        Move();
    }


    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
           transform.position.z);
        Gizmos.DrawSphere(spherePosition, GroundedRadius);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        Vector3 targetRotation = new Vector3(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        cameraRoot.rotation = Quaternion.Euler(targetRotation);
        foreach (var c in camerasRootFollow) c.rotation = Quaternion.Euler(targetRotation);

        //Aiming Point
        Ray ray = _mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            aimObj.position = hit.point;
        }
        else
        {
            aimObj.position = cineCamTarget.position + cineCamTarget.forward * aimDistance;
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            if (IsFollowCameraRotation())
            {
                _animator.SetFloat("Horizontal Movement", Mathf.Lerp(_animator.GetFloat("Horizontal Movement"), _input.move.x / 2, Time.deltaTime * SpeedChangeRate));
                _animator.SetFloat("Vertical Movement", Mathf.Lerp(_animator.GetFloat("Vertical Movement"), _input.move.y / 2, Time.deltaTime * SpeedChangeRate));
            }
            else
            {
                _animator.SetFloat("Horizontal Movement", Mathf.Lerp(_animator.GetFloat("Horizontal Movement"), 0, Time.deltaTime * SpeedChangeRate));
                _animator.SetFloat("Vertical Movement", Mathf.Lerp(_animator.GetFloat("Vertical Movement"), 1, Time.deltaTime * SpeedChangeRate));
            }

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCameraTransform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if (!IsFollowCameraRotation())
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        Move(targetDirection.normalized * _speed * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        if (IsFollowCameraRotation())
            transform.rotation = Quaternion.Euler(0,
               Mathf.LerpAngle(transform.rotation.eulerAngles.y, _cinemachineTargetYaw, RotateSpeed * Time.deltaTime),
                0);
    }

    protected float hig = 0;
    private void Jump()
    {
        if (rigidbody.velocity.y > hig)
            hig = rigidbody.velocity.y;
        print("HIg=" + hig);
        if (Grounded)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);
            // Jump
            if (_input.jump)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
                rigidbody.velocity += new Vector3(0, _verticalVelocity, 0);
                _input.jump = false;
            }

        }
        else
        {
            _animator.SetBool(_animIDFreeFall, true);
            // if we are not grounded, do not jump
            _input.jump = false;
        }


    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(footAudioPos.position), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(footAudioPos.position), FootstepAudioVolume);
        }
    }

    private bool IsFollowCameraRotation()
    {
        if (isFpcam) return true;
        if (equipRifle)
        {
            if (isRifleReloading) return false;
            if (isRifleAiming) return true;
            if (isRifleFiring) return true;
        }
        else if (equipPistol)
        {
            if (isPistolReloading) return false;
            if (isPistolAiming) return true;
            if (isPistolFiring) return true;
        }
        if (isRotatePlayerWithCam) return true;

        return false;
    }

    private void InitCameraView()
    {
        tpCam.Priority = 10;
        isFpcam = true;
        ChangeView();
        _input.onChangeView.AddListener(ChangeView);
    }

    public void ChangeView()
    {
        if (!isFpcam)
        {
            fpCam.Priority = 13;
            _mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Body"));
            isRotatePlayerWithCam = true;
            isFpcam = true;
            aimRig.SmoothRig(1);
        }
        else
        {
            fpCam.Priority = 8;
            _mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Body");
            isRotatePlayerWithCam = false;
            isFpcam = false;
            aimRig.SmoothRig(0);
        }
    }

    public override void StartRifleAimAnimation()
    {
        base.StartRifleAimAnimation();
        tpAimCam.Priority = 11;
    }

    public override void StopRifleAimAnimation()
    {
        base.StopRifleAimAnimation();
        tpAimCam.Priority = 9;
    }

    public override void StartPistolAimAnimtion()
    {
        base.StartPistolAimAnimtion();
        tpAimCam.Priority = 11;
    }

    public override void StopPistolAimAnimation()
    {
        base.StopPistolAimAnimation();
        tpAimCam.Priority = 9;
    }
}