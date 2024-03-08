using UnityEngine;

[RequireComponent(typeof(PlayerMovementComponent))]
[RequireComponent(typeof(AnimatorController))]
public class Player : MonoBehaviour
{
    private PlayerMovementComponent movementComponent;
    private AnimatorController animatorController;

    public PlayerMovementComponent GetMovementComponent() 
    { 
        return movementComponent;
    }

    public void Fire()
    {
        animatorController.RunShootAnimation();
    }

    public void Move(Vector2 input)
    {
        movementComponent.SetMoveInput(input);
    }

    public void StopMove()
    {
        movementComponent.SetMoveInput(Vector2.zero);
    }

    public void LookAtPoint(Vector2 inputPoint)
    {
        movementComponent.SetAimInput(inputPoint);
    }

    public void StopLooking()
    {
        movementComponent.SetAimInput(Vector2.zero);
    }

    public void StartRunning()
    {
        movementComponent.SetMovementSpeed(movementComponent.GetRunSpeed());
        movementComponent.SetRunningKey(true);
    }

    public void StopRunning()
    {
        movementComponent.SetMovementSpeed(movementComponent.GetWalkSpeed());
        movementComponent.SetRunningKey(false);
    }

    private void Start()
    {
        movementComponent = GetComponent<PlayerMovementComponent>();
        animatorController = GetComponent<AnimatorController>();
    }
}
