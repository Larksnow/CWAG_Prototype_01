using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MouseManager : MonoBehaviour
{
    public RectTransform crosshair;
    // Update is called once per frame
    void Update()
    {
        // Print out the position of the mouse
        // When the mouse moves, print out the position of the mouse
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            UpdatePointerPosition();
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.F4))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space ))
        {
            CameraManager.main.MoveCamera();
        }

    }

    public void UpdatePointerPosition()
    {
            
        // Convert the mouse position to a canvas position
        float scaling =  1920f / Screen.width;
        Vector2 canvasPosition = Input.mousePosition * scaling;
            
        canvasPosition.x = canvasPosition.x - (Screen.width / 2) * scaling;
        canvasPosition.y = canvasPosition.y - (Screen.height / 2) * scaling;
        
        UpdateCrosshairPostiton(canvasPosition);
    }

    public void UpdateCrosshairPostiton(Vector2 screenPosition)
    {
        crosshair.anchoredPosition = screenPosition;
    }

    public Vector2 GetCrosshairPosition()
    {
        return crosshair.anchoredPosition;
    }
}
