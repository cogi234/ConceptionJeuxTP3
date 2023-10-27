using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour, IMouseSensitivity
{
    float mouseSensibility = 10;
    [SerializeField] float currentAngle = 0;
    float IMouseSensitivity.MouseSensitivity { set { mouseSensibility = value; } }

    void LateUpdate()
    {
        //To avoid the camera fucking around when the game starts
        if (Time.time < 0.1f)
            return;

        currentAngle = Mathf.Clamp(currentAngle - (Mouse.current.delta.y.ReadValue() * mouseSensibility * Time.deltaTime), -90, 90);
        transform.localRotation = Quaternion.Euler(currentAngle, 0, 0);
    }
}
