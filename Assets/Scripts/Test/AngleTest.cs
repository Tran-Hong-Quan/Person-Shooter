using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    [SerializeField] Vector3 axis;
    [SerializeField] float angle;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.forward = Quaternion.AngleAxis(angle, axis) * transform.forward;
            Debug.Log(transform.forward);
        }
    }
}
