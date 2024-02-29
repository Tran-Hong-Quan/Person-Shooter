using HongQuan;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] protected Transform firePoint;
    [SerializeField, Tooltip("1 Bullet duration")] protected float fireRate = 0.1f;
    [SerializeField] protected float muzzleVelocity = 0.1f;
    [SerializeField] protected Vector2 recoilForce;
    [SerializeField] protected float recoilRecovery;

    [SerializeField] protected ParticleSystem fireEffect;
    [SerializeField] protected ParticleSystem bulletHitEff;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireAudioClip;

    protected PlayerInputs inputs;
    protected Camera mainCam;
    protected Animator animator;
    protected ProceduralRecoil recoil;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        inputs = playerController.Inputs;

        inputs.onAim.AddListener(Aim);

        inputs.onFire.AddListener(Fire);
        inputs.onStartFire.AddListener(StartFire);
        inputs.onStopFire.AddListener(StopFire);

        animator = playerController.Animator;
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
        if (clk > 0) clk -= Time.fixedDeltaTime;

        recoilValue = Vector3.Slerp(recoilForce, Vector3.zero, Time.deltaTime * recoilRecovery);
    }

    private void StartFire()
    {
        playerController.StartRifleFireAnimation();
    }

    private void StopFire()
    {
        playerController.StopRifleFireAnimation();
    }

    private void Fire()
    {
        if (clk > 0f) return;

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

        float recoilMulti = 1f;
        if (inputs.sprint) recoilMulti *= 2f;
        if (inputs.move != Vector2.zero) recoilMulti *= 1.5f;
        if (!isAiming) recoilMulti *= 1.2f;
        recoilValue += new Vector2(Random.Range(0, recoilForce.x), Random.Range(-recoilForce.y, recoilForce.y)) * recoilMulti;
        playerController.AddCinemachineCamRotatation(-recoilValue.x, recoilValue.y);

        audioSource.PlayOneShot(fireAudioClip);

        animator.Play("Rifle Fire", animator.GetLayerIndex("Rifle Fire"));

        clk = fireRate;
    }

    Vector2 recoilValue = Vector2.zero;

    bool isAiming;

    private void StartAim()
    {
        isAiming = true;
        playerController.StartRifleAimAnimation();
    }

    private void StopAim()
    {
        isAiming = false;
        playerController.StopRifleAimAnimation();
    }

    private void OnDestroy()
    {

    }
}
