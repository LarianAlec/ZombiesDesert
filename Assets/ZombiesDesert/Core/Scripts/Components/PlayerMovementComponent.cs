using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MonoBehaviour
{
    private CharacterController characterController;

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

    public void SetMoveInput(Vector2 inputVector)
    {
        moveInput = inputVector;
    }

    public void SetAimInput(Vector2 inputVector)
    {
        aimInput = inputVector;
    }

    public void SetRunningKey(bool inputKey)
    { 
        isRunningKeyPressed = inputKey;
    }

    public void SetMovementSpeed(float newSpeed)
    { 
        speed = newSpeed;
    }

    public Vector3 GetMovementDirection()
    {
        return movementDirection;
    }

    public float GetWalkSpeed()
    {  
        return walkSpeed;
    }

    public float GetRunSpeed()
    { 
        return runSpeed;
    }

    public bool IsRunningKeyPressed()
    { 
        return isRunningKeyPressed;
    }

    public float GetXVelocity()
    {
        return Vector3.Dot(movementDirection.normalized, transform.right);
    }
 
    public float GetZVelocity()
    {
        return Vector3.Dot(movementDirection.normalized, transform.forward);
    }

    private void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();

        speed = walkSpeed;
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardMousePositon();
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

}
