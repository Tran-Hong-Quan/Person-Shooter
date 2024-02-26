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
            
        }
        else
        {
            isAiming = true;
        }
    }
}
