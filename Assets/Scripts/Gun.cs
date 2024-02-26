using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    
    PlayerInputs inputs;
    Animator animator;
    Cinemachine.CinemachineVirtualCamera tpCam;
    Cinemachine.CinemachineVirtualCamera fpCam;

    private void Start()
    {
        animator = playerController.Animator;
        inputs = playerController.Input;
        tpCam = playerController.tpCam;
        fpCam = playerController.fpCam;
        inputs.onAim.AddListener(Aim);
    }

    private void Update()
    {

    }
    bool isAiming;

    private void Aim()
    {
        if (isAiming)
        {
            isAiming = false;
            playerController.ChangeView();
            animator.SetLayerWeight(animator.GetLayerIndex("Aim"), 0);
        }
        else
        {
            isAiming = true;
            playerController.ChangeView();
            animator.SetLayerWeight(animator.GetLayerIndex("Aim"),1);
        }
    }
}
