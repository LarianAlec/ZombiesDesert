using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerMovementComponent))]
[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterEquipmentComponent))]
public class BaseCharacter : MonoBehaviour
{
    public TopDownAimComponent aim { get; private set; }

    protected CharacterEquipmentComponent characterEquipmentComponent;
    protected PlayerMovementComponent movementComponent;
    protected AnimatorController animInstance { get; private set; }

    protected virtual void Awake()
    {
        aim = GetComponent<TopDownAimComponent>();
        movementComponent = GetComponent<PlayerMovementComponent>();
        characterEquipmentComponent = GetComponent<CharacterEquipmentComponent>();
        animInstance = GetComponentInChildren<AnimatorController>();
    }

    public virtual void Move(Vector2 input) { }

    public virtual void StopMove() { }

    public virtual void LookAtPoint(Vector2 inputPoint) { }

    public virtual void StopLooking() { }

    public void StartFire() 
    {
        if (characterEquipmentComponent.IsEquipping())
        {
            return;
        }
        Weapon currentWeapon = characterEquipmentComponent.GetCurrentEquippedWeapon();
        if (currentWeapon != null)
        {
            currentWeapon.StartFire();
        }
    }

    public void StopFire() 
    {
        Weapon currentRangeWeapon = characterEquipmentComponent.GetCurrentEquippedWeapon();
        if (currentRangeWeapon != null)
        {
            currentRangeWeapon.StopFire();
        }
    }

    public void Reload()
    {
        if (characterEquipmentComponent.GetCurrentEquippedWeapon() != null)
        {
            characterEquipmentComponent.ReloadCurrentWeapon();
        }
    }

    public void EquipItemInSlot(int slotIndex)
    {
        characterEquipmentComponent.EquipItemInSlot((EquipmentSlots)slotIndex);
    }

    public void NextItem() 
    {
        characterEquipmentComponent.EquipNextItem();
    }

    public void PreviousItem() 
    {
        characterEquipmentComponent.EquipPreviousItem();
    }

    public CharacterEquipmentComponent GetEquipmentComponent() 
    { 
        return characterEquipmentComponent;
    }

    public PlayerMovementComponent GetMovementComponent() 
    { 
        return movementComponent;
    }

    public void ChangeAimingState()
    {
        if (aim.IsAimingPrecisely())
        {
            aim.SetPreciselyAiming(false);
        }
        else
        {
            aim.SetPreciselyAiming(true);
        }
    }

    public void SwitchTargetLockingState()
    {
        if (aim.IsTargetLockingEnabled())
        {
            aim.DisableTargetLocking();
        }
        else
        {
            aim.EnableTargetLocking();
        }
    }

    public AnimatorController GetAnimInstance() => animInstance;
}
