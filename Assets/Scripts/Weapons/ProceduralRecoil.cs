using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    public Vector2 recoilForce = Vector2.one;
    public float recoilRecoverySpeed = 2f;

    private Transform _transform;
    private Vector3 targetRotation;
    private Vector3 currentRotation;

    void Start()
    {
        _transform = transform;
    }

    public void Init(Vector3 recoilForce,float recoilRecoverySpeed)
    {
        this.recoilForce = recoilForce;
        this.recoilRecoverySpeed = recoilRecoverySpeed;
    }


    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilRecoverySpeed * Time.deltaTime);
        _transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Aim()
    {
        targetRotation += GetRecoilValue();
    }

    private Vector3 GetRecoilValue()
    {
        return new Vector3(recoilForce.x, Random.Range(-recoilForce.y, recoilForce.y), Random.Range(-recoilForce.y, recoilForce.y));
    }
}
