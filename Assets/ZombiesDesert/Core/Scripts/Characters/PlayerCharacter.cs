using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    public HealthController_Player health {  get; private set; }

    public RagdollComponent ragdoll { get; private set; }

    private bool bIsRunning = false;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<HealthController_Player>();
        ragdoll = GetComponent<RagdollComponent>();
    }

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

    // =================== DEBUG SECTION ==========================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            health.ReduceHealth();
        }
    }

}
