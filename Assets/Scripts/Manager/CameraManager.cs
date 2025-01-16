using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager main;
    public List<Transform> cameraPositions;
    private int index = 0;
    public float transitionSpeed = 2.0f; // Speed of the camera transition

    private void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }

    public void MoveCamera()
    {
        index = (index + 1) % cameraPositions.Count;
        Transform currentTarget = cameraPositions[index];

        // Smoothly move the camera to the new position and rotation
        StartCoroutine(SmoothMoveCamera(currentTarget));
    }

    private IEnumerator SmoothMoveCamera(Transform target)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            // Interpolate position and rotation
            Camera.main.transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, target.rotation, elapsedTime);

            yield return null;
        }

        // Ensure the camera is exactly at the target position and rotation at the end
        Camera.main.transform.position = target.position;
        Camera.main.transform.rotation = target.rotation;
    }
}
