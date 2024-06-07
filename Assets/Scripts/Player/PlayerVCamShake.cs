using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVCamShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] vCam;
    public void StartShake(float amplitude = 2, float frequency = 2, float fadeDuration = 0.2f)
    {
        foreach (var cam in vCam)
        {
            StartShakeCam(cam, amplitude, frequency, fadeDuration);
        }
    }

    public void StopShake(float duration = 0.2f)
    {
        foreach (var cam in vCam)
        {
            StopShakeCam(cam, duration);
        }
    }

    public void StopShakeCam(CinemachineVirtualCamera cam, float fadeDuration)
    {
        var shake = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        DOVirtual.Float(shake.m_AmplitudeGain, 0, fadeDuration, value =>
        {
            shake.m_AmplitudeGain = value;
        });
        DOVirtual.Float(shake.m_FrequencyGain, 0, fadeDuration, value =>
        {
            shake.m_FrequencyGain = value;
        });
    }

    public void StartShakeCam(CinemachineVirtualCamera cam, float amplitude, float frequency, float duration)
    {
        var shake = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        DOVirtual.Float(shake.m_AmplitudeGain, amplitude, duration, value =>
        {
            shake.m_AmplitudeGain = value;
        });
        DOVirtual.Float(shake.m_FrequencyGain, frequency, duration, value =>
        {
            shake.m_FrequencyGain = value;
        });
    }
}
