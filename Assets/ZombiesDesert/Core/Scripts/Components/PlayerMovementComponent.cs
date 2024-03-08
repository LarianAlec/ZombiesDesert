using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement info")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float gravityScale = 9.81f;
    private float speed;
    private Vector3 movementDirection;
    private float verticalVelocity;

    [Header("Aim info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookDirection;

    private Vector2 moveInput;
    private Vector2 aimInput;
    private bool isRunningKeyPressed;

    public Vector3 GetMovementDirection()
    {
        return movementDirection;
    }
    
    public bool IsRunningKeyPressed()
    { return isRunningKeyPressed; }


    private void Awake()
    {
        controls = new PlayerControls();

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunningKeyPressed = true;
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunningKeyPressed = false;
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardMousePositon();
        //HandleAnimations();
    }

    private void AimTowardMousePositon()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f;
            lookDirection.Normalize();

            transform.forward = lookDirection;

            aim.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * speed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
        }
        else
        {
            verticalVelocity = -0.05f;
        }
        movementDirection.y = verticalVelocity;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    /*
    private void HandleAnimations()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool isRunning = movementDirection.magnitude > 0.15f && isRunningKeyPressed;
        animator.SetBool("isRunning", isRunning);
    }
    */
}
