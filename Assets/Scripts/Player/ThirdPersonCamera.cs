using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour, IMouseSensitivity
{
    //Code copié et adapté de mon PFI de la session passée (Colin)

    [SerializeField] Transform targetTransform;
    [SerializeField] float idealDistance;
    [SerializeField] float pitchOffset;
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float distanceFromCollision;
    [SerializeField] float defaultAngle;

    float angleFromTarget = 25;
    float mouseSensibility;

    Vector3 targetPosition;
    Quaternion targetRotation;

    public float MouseSensitivity { set { mouseSensibility = value; } }

    private void Awake()
    {
        angleFromTarget = defaultAngle;
    }

    void LateUpdate()
    {
        angleFromTarget += mouseSensibility * -Mouse.current.delta.y.ReadValue() * Time.deltaTime;
        angleFromTarget = Mathf.Clamp(angleFromTarget, minAngle, maxAngle);

        Quaternion rotation = Quaternion.Euler(angleFromTarget, 0, 0);
        Vector3 raycastWorldDirection = targetTransform.localToWorldMatrix * (rotation * -Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(targetTransform.position, raycastWorldDirection, out hit, idealDistance))
        {
            targetPosition = hit.point - raycastWorldDirection.normalized * distanceFromCollision;
        }
        else
        {
            targetPosition = targetTransform.position + (raycastWorldDirection.normalized * idealDistance);
        }

        targetRotation = Quaternion.LookRotation(-raycastWorldDirection, transform.up) * Quaternion.Euler(pitchOffset, 0, 0);


        //On bouge la camera doucement vers la position et rotation necessaire
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.75f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.75f);
    }
}
