using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCam : MonoBehaviour
{
    Transform mainCamTransform;
    Transform _transform;
    void Start()
    {
        _transform = transform;
        mainCamTransform = Camera.main.transform;
    }


    void Update()
    {
        _transform.LookAt(transform.position + mainCamTransform.rotation * Vector3.forward, 
            mainCamTransform.rotation*Vector3.up);
    }
}
