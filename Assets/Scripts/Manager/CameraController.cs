using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Singleton object
    public static CameraController main;
    public CinemachineVirtualCamera virtualCamera;

    int currAngle = 2;
    float[] angles = {0, 90, 180, 270};
    [SerializeField] float rotationTime = 0.2f;
    
    private void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Turn(isRight:false);
        if (Input.GetKeyDown(KeyCode.D))
            Turn(isRight:true);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Turn(isRight:false);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Turn(isRight:true);
    }

    public void Turn(bool isRight)
    {
        Vector3 currRotation = transform.rotation.eulerAngles;
        currAngle = (currAngle + (isRight ? 1 : -1)) % 4;
        if (currAngle == -1) currAngle = 3;
        transform.DORotate(new Vector3(currRotation.x, angles[currAngle], 0), rotationTime);
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
