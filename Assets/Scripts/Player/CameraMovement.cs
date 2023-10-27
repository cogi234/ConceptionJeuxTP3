using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] List<Transform> cameraPoints = new List<Transform>();
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] Transform player;

    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float transitionTime = 0.5f;

    [SerializeField] float mouseSensitivity = 10;

    int currentCameraPoint = 0;
    Coroutine currentAction;

    private void Awake()
    {
        //Input stuff
        InputActionMap inputMap = inputAsset.FindActionMap("player");
        InputAction cameraSwitch = inputMap.FindAction("cameraSwitch");
        cameraSwitch.performed += SwitchCamera;
        //set the vertical mouse sensitivity
        cameraPoints[currentCameraPoint].GetComponent<IMouseSensitivity>().MouseSensitivity = mouseSensitivity;
    }

    private void Start()
    {
        //On commence avec un seul point activer
        for (int i = cameraPoints.Count - 1; i > 0; i--)
        {
            cameraPoints[i].gameObject.SetActive(false);
        }

        //On initialise au point de la premiere camera
        currentAction = StartCoroutine(Follow(cameraPoints[0]));
    }

    private void SwitchCamera(InputAction.CallbackContext action)
    {
        StopCoroutine(currentAction);

        cameraPoints[currentCameraPoint].gameObject.SetActive(false);
        currentCameraPoint = (currentCameraPoint + 1) % cameraPoints.Count;
        cameraPoints[currentCameraPoint].gameObject.SetActive(true);
        cameraPoints[currentCameraPoint].GetComponent<IMouseSensitivity>().MouseSensitivity = mouseSensitivity;

        currentAction = StartCoroutine(Transition(cameraPoints[currentCameraPoint]));
    }
    public void SwitchCamera(int index)
    {
        StopCoroutine(currentAction);

        cameraPoints[currentCameraPoint].gameObject.SetActive(false);
        currentCameraPoint = Mathf.Clamp(index, 0, cameraPoints.Count - 1);
        cameraPoints[currentCameraPoint].gameObject.SetActive(true);
        cameraPoints[currentCameraPoint].GetComponent<IMouseSensitivity>().MouseSensitivity = mouseSensitivity;

        currentAction = StartCoroutine(Transition(cameraPoints[currentCameraPoint]));
    }

    IEnumerator Transition(Transform target)
    {
        float timer = 0;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, timer / transitionTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, timer / transitionTime);
            yield return null;
        }

        currentAction = StartCoroutine(Follow(target));
    }

    IEnumerator Follow(Transform target)
    {
        while (true)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        //tourne horizontalement le personnage
        player.Rotate(new Vector3(0, Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.fixedDeltaTime, 0));
    }
}
