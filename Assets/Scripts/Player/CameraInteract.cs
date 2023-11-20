using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class CameraInteract : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] Transform player;
    [SerializeField] GameObject interactText;
    [SerializeField] float interactionDistance = 5;
    [Header("Animation stuff")]
    [SerializeField] Rig rig;
    [SerializeField] Transform target;
    [SerializeField] float timeToInteract = 0.5f;


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
            StartCoroutine(AnimateInteraction(hit.transform));
        }
    }

    private IEnumerator AnimateInteraction(Transform altar)
    {
        target.position = altar.position + new Vector3(0, 1, 0);
        //reach out
        float timer = 0;
        while (timer < timeToInteract)
        {
            timer += Time.deltaTime;
            rig.weight = Mathf.Min(timer / timeToInteract, 1);
            yield return null;
        }
        //stop reaching out
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            rig.weight = Mathf.Max(timer / timeToInteract, 0);
            yield return null;
        }
    }
}
