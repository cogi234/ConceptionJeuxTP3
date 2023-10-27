using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] float speed = 5;
    [SerializeField] float sprintMultiplier = 2;
    [SerializeField] float jumpSpeed = 5;

    CharacterController characterController;
    bool sprinting = false;
    Vector2 moveDirection = new Vector2();
    float ySpeed = 0;
    float originalStepOffset;

    //Animation stuff
    Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponentInChildren<Animator>();

        //Input setup
        InputActionMap playerInput = inputAsset.FindActionMap("player");
        InputAction movementInput = playerInput.FindAction("movement");
        InputAction jumpInput = playerInput.FindAction("jump");
        InputAction sprintInput = playerInput.FindAction("sprint");
        movementInput.performed += MoveCall;
        movementInput.canceled += MoveCall;
        jumpInput.performed += JumpCall;
        jumpInput.canceled += JumpStop;
        sprintInput.performed += SprintCall;
    }

    private void MoveCall(InputAction.CallbackContext action)
    {
        moveDirection = action.action.ReadValue<Vector2>().normalized;
        //Si on n'avance pas, on arrete de courir
        if (moveDirection.y <= 0)
        {
            sprinting = false;
        }
    }
    private void JumpCall(InputAction.CallbackContext action)
    {
        //On saute seulement si on touche au sol
        if (characterController.isGrounded)
        {
            ySpeed = jumpSpeed;
        }
    }
    private void JumpStop(InputAction.CallbackContext action)
    {
        //Lorsque on lache le bouton de saut, on arrete la vitesse verticale
        ySpeed = Mathf.Min(0, ySpeed);
    }
    private void SprintCall(InputAction.CallbackContext action)
    {
        sprinting = true;
    }

    private void FixedUpdate()
    {
        //Gravite
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (characterController.isGrounded)
        {
            ySpeed = Mathf.Max(-0.5f, ySpeed);
            characterController.stepOffset = originalStepOffset;
        } else
        {
            characterController.stepOffset = 0;
        }

        //On execute le mouvement
        Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * speed;
        //Sprint:
        movement *= sprinting ? sprintMultiplier : 1;

        //Animator stuff
        animator.SetBool("Going Right", movement.x > 0);
        animator.SetFloat("Side Speed Abs", Mathf.Abs(movement.x));
        animator.SetFloat("Speed", movement.y);

        //On transforme en mouvement relatif a notre rotation
        movement = transform.localToWorldMatrix * movement;
        //Mouvement vertical
        movement.y = ySpeed;

        characterController.Move(movement * Time.fixedDeltaTime);
    }
}
