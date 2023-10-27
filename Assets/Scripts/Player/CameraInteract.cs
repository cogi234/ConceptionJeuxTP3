using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInteract : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] Transform player;
    [SerializeField] GameObject interactText;
    [SerializeField] float interactionDistance = 5;

    private void Awake()
    {
        InputActionMap playerInput = inputAsset.FindActionMap("player");
        InputAction interactInput = playerInput.FindAction("interact");
        interactInput.performed += TryInteract;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 25, LayerMask.GetMask("Interactable")) && hit.transform.tag == "Interactable" && Vector3.Distance(hit.point, player.position) < interactionDistance)
        {
            if (!interactText.activeInHierarchy)
            {
                //If we hit an interactable and the interact text isn't active, we activate it;
                interactText.SetActive(true);
            }
        } else if (interactText.activeInHierarchy)
        {
            //If there's no interactable but the text is active, deactivate it
            interactText.SetActive(false);
        }
    }

    private void TryInteract(InputAction.CallbackContext action)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 25, LayerMask.GetMask("Interactable")) && hit.transform.tag == "Interactable" && Vector3.Distance(hit.point, player.position) < interactionDistance)
        {
            hit.transform.GetComponent<IInteractable>().Interact();
        }
    }
}
