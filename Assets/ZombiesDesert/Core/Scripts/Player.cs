using UnityEngine;

[RequireComponent(typeof(PlayerMovementComponent))]
[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerWeaponController))]
public class Player : MonoBehaviour
{
    private PlayerMovementComponent movementComponent;
    private AnimatorController animatorController;
    private PlayerWeaponController weaponController;

    public PlayerMovementComponent GetMovementComponent() 
    { 
        return movementComponent;
    }

    public void Fire()
    {
        weaponController.Shoot();
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

    public void EquipFirstSlot()
    {
        GetComponentInChildren<WeaponVisualComponent>().SwitchOnFirstSlot();
    }

    public void EquipSecondSlot()
    {
        GetComponentInChildren<WeaponVisualComponent>().SwitchOnSecondSlot();
    }

    public void EquipThirdSlot()
    {
        GetComponentInChildren<WeaponVisualComponent>().SwitchOnThirdSlot();
    }

    private void Start()
    {
        movementComponent = GetComponent<PlayerMovementComponent>();
        animatorController = GetComponent<AnimatorController>();
        weaponController = GetComponent<PlayerWeaponController>();
    }
}
