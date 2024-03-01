using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    public Vector3 recoilForce = Vector3.one;
    public float recoverySpeed = 2f;
    public float snappiness = 2f;

    public List<Transform> targets;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    public void Init(List<Transform> targets)
    {
        this.targets = targets;
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoverySpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoverySpeed * Time.deltaTime);

        foreach (Transform t in targets)
        {
            t.localRotation = Quaternion.Euler(currentRotation);
        }
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
