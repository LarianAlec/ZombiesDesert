
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    
    private bool bIsRunning = false;

    public bool IsRunning() => bIsRunning;

    public override void Move(Vector2 input)
    {
        movementComponent.SetMoveInput(input);
    }

    public override void StopMove()
    {
        movementComponent.SetMoveInput(Vector2.zero);
    }

    public override void LookAtPoint(Vector2 inputPoint)
    {
        aim.SetAimInput(inputPoint);
    }

    public override void StopLooking()
    {
        aim.SetAimInput(Vector2.zero);
    }

    public void StartRunning()
    {
        movementComponent.StartRunning();
    }

    public void StopRunning()
    {
        movementComponent.StopRunning();
    }
}
