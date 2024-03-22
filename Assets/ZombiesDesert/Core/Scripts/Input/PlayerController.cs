using UnityEngine;

[RequireComponent(typeof(BaseCharacter))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerCharacter player;

    public PlayerControls GetControls() 
    { 
        return controls; 
    }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        player = GetComponent<PlayerCharacter>();

        AssignInputEvents();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void AssignInputEvents()
    {
        controls.Character.Movement.performed += context => player.Move(context.ReadValue<Vector2>());
        controls.Character.Movement.canceled += context => player.StopMove();

        controls.Character.Aim.performed += context => player.LookAtPoint(context.ReadValue<Vector2>());
        controls.Character.Aim.canceled += context => player.StopLooking();

        controls.Character.Run.performed += context => player.StartRunning();
        controls.Character.Run.canceled += context => player.StopRunning();

        controls.Character.Fire.performed += context => player.StartFire();
        controls.Character.Fire.canceled += context => player.StopFire();

        controls.Character.AimPrecisely.performed += context => player.ChangeAimingState();

        controls.Character.TargetLocking.performed += context => player.SwitchTargetLockingState();

        controls.Character.Reload.performed += context => player.Reload();

        controls.Character.EquipItem.performed += context =>
        {
            if (context.ReadValue<float>() > 0)
                player.NextItem();
            else
                player.PreviousItem();
        };

        controls.Character.EquipFirstSlot.performed += context => player.EquipItemInSlot(1);
        controls.Character.EquipSecondSlot.performed += context => player.EquipItemInSlot(2);
        controls.Character.EquipThirdSlot.performed += context => player.EquipItemInSlot(3);
        controls.Character.EquipFourthSlot.performed += context => player.EquipItemInSlot(4);
    }
}
