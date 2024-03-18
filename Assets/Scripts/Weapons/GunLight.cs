using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunLight : MonoBehaviour
{
    public float duration = 0.1f;

    private Light _light;
    private void Awake()
    {
        _light = GetComponent<Light>();
        _light.enabled = false;
    }

    public void Flick()
    {
        
    }

}
