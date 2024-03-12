using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] Transform follow;
    Transform _transform;
    void Start()
    {
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!follow) return;

        transform.position = follow.position;
        transform.rotation = follow.rotation;
    }
}
