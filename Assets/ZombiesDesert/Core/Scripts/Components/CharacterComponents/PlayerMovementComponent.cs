using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MonoBehaviour
{
    private PlayerCharacter player;
    private CharacterController characterController;

    [Header("Movement info")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float rotaionRate = 12.0f;
    [SerializeField] private float gravityScale = 9.81f;
    private float speed;
    private Vector3 movementDirection;
    private float verticalVelocity;

    private Vector2 moveInput;
    private bool isRunningKeyPressed;

    public void StartRunning()
    {
        speed = runSpeed;
        isRunningKeyPressed = true;
    }

    public void StopRunning()
    {
        speed = walkSpeed;
        isRunningKeyPressed = false;
    }

    public void SetMoveInput(Vector2 inputVector)
    {
        moveInput = inputVector;
    }

    public Vector3 GetMovementDirection()
    {
        return movementDirection;
    }

    public float GetXVelocity()
    {
        return Vector3.Dot(movementDirection.normalized, transform.right);
    }
 
    public float GetZVelocity()
    {
        return Vector3.Dot(movementDirection.normalized, transform.forward);
    }

    public bool IsRunningKeyPressed()
    { 
        return isRunningKeyPressed;
    }

    private void Start()
    {
        player = GetComponent<PlayerCharacter>();
        characterController = GetComponent<CharacterController>();

        speed = walkSpeed;
    }

    private void Update()
    {
        if (player.health.isDead)
            return;

        ApplyMovement();
        ApplyRotation();
    }
    
    private void ApplyRotation()
    {
        Vector3 lookingDirection = player.aim.GetMousePosition() - transform.position;
        lookingDirection.y = 0.0f;
        lookingDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotaionRate * Time.deltaTime);
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
