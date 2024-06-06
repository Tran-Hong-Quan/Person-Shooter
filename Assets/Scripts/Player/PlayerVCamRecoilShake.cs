using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVCamRecoilShake : MonoBehaviour
{
    public CinemachineVirtualCamera[] vcams;
    public float fallBackSpeed = 4;
    public float frequency = 4;
    public float amplitude = 1;


    private float currentFrequency;
    private float currentAmplitude;
    private CinemachineBasicMultiChannelPerlin[] shakeCam;
    private void Awake()
    {
        shakeCam = new CinemachineBasicMultiChannelPerlin[vcams.Length];
        for (int i = 0; i < vcams.Length; i++)
        {
            shakeCam[i] = vcams[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void Shake(float frequency,float amplitude)
    {
        currentFrequency = frequency;
        currentAmplitude = amplitude;
    }
    public void Shake()
    {
        currentFrequency = frequency;
        currentAmplitude = amplitude;
    }

    private void Update()
    {
        currentFrequency = Mathf.Lerp(currentFrequency, 0, Time.deltaTime * fallBackSpeed);
        currentAmplitude = Mathf.Lerp(currentAmplitude, 0, Time.deltaTime * fallBackSpeed);
        SetShake(currentFrequency, currentAmplitude);
    }

    private void SetShake(float frequency, float amplitude)
    {
        foreach (var s in shakeCam)
        {
            s.m_FrequencyGain = frequency;
            s.m_AmplitudeGain = amplitude;
        }
    }
}
