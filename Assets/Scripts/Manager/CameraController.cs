using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Singleton object

    public static CameraController main;
    public CinemachineVirtualCamera virtualCamera;
    
    private void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));    
    }
    
    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        // move the camera forwards and backward
        CinemachineBasicMultiChannelPerlin perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = magnitude;
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0;
    }
}
