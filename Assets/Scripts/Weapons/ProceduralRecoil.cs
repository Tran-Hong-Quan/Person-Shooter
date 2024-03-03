using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    public Vector3 recoilForce = Vector3.one;
    public float recoverySpeed = 2f;
    public float snappiness = 2f;

    public Vector3 maxRecoil = new Vector3(12, 9, 9);

    public List<Transform> targets;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    public void Init(List<Transform> targets)
    {
        this.targets = targets;
    }

    public void ClearTargets()
    {
        targets.Clear();
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoverySpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoverySpeed * Time.deltaTime);
        currentRotation = new Vector3(
            Mathf.Clamp(currentRotation.x, -maxRecoil.x, 0),
            Mathf.Clamp(currentRotation.y, -maxRecoil.y, maxRecoil.y),
            Mathf.Clamp(currentRotation.z, -maxRecoil.z, maxRecoil.z));

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
        return new Vector3(-recoilForce.x,
            Random.Range(-recoilForce.y, recoilForce.y),
            Random.Range(-recoilForce.z, recoilForce.z)) * Time.deltaTime;
    }
}
