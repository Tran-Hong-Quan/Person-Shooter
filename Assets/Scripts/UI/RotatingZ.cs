using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingZ : MonoBehaviour
{
    public float speed;
    public float dir;

    public Transform _transform;

    void Start()
    {
        _transform = transform;
        if (dir > 0) dir = 1; else dir = -1;
    }

    void Update()
    {
        _transform.Rotate(Vector3.forward, speed);
    }
}
