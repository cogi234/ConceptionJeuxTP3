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
    [SerializeField] float lerpConstant = 5;

    CharacterController characterController;
    bool sprinting = false;
    Vector2 moveDirection = new Vector2();
    Vector3 moveSpeed = new Vector3();
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
            moveSpeed.y = jumpSpeed;
            animator.SetTrigger("Jump");
        }
    }
    private void JumpStop(InputAction.CallbackContext action)
    {
        //Lorsque on lache le bouton de saut, on arrete la vitesse verticale
        moveSpeed.y = Mathf.Min(0, moveSpeed.y);
    }
    private void SprintCall(InputAction.CallbackContext action)
    {
        sprinting = true;
    }

    private void FixedUpdate()
    {
        //Gravite
        moveSpeed.y += Physics.gravity.y * Time.fixedDeltaTime;
        if (characterController.isGrounded)
        {
            moveSpeed.y = Mathf.Max(-0.5f, moveSpeed.y);
            characterController.stepOffset = originalStepOffset;
        } else
        {
            characterController.stepOffset = 0;
        }

        //On execute le mouvement
        Vector3 targetSpeed = new Vector3(moveDirection.x, 0, moveDirection.y) * speed;
        //Sprint:
        targetSpeed *= sprinting ? sprintMultiplier : 1;
        //Mouvement vertical
        targetSpeed.y = moveSpeed.y;

        //Interpolation de la vitesse vers notre cible
        moveSpeed = Vector3.Lerp(moveSpeed, targetSpeed, Time.fixedDeltaTime * lerpConstant);

        //Animator stuff
        animator.SetBool("Grounded", characterController.isGrounded);
        animator.SetFloat("X", moveSpeed.x);
        animator.SetFloat("Z", moveSpeed.z);

        //On transforme en mouvement relatif a notre rotation
        Vector3 movement = transform.localToWorldMatrix * moveSpeed;

        characterController.Move(movement * Time.fixedDeltaTime);
    }
}
