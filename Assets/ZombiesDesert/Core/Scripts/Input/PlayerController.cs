using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        if (playerCharacter == null)
        {
            playerCharacter = FindObjectOfType<PlayerCharacter>();
        }
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
        controls.Character.Movement.performed += context => playerCharacter.Move(context.ReadValue<Vector2>());
        controls.Character.Movement.canceled += context => playerCharacter.StopMove();

        controls.Character.Aim.performed += context => playerCharacter.LookAtPoint(context.ReadValue<Vector2>());
        controls.Character.Aim.canceled += context => playerCharacter.StopLooking();

        controls.Character.Run.performed += context => playerCharacter.StartRunning();
        controls.Character.Run.canceled += context => playerCharacter.StopRunning();

        controls.Character.Fire.performed += context => playerCharacter.StartFire();
        controls.Character.Fire.canceled += context => playerCharacter.StopFire();

        controls.Character.AimPrecisely.performed += context => playerCharacter.ChangeAimingState();

        controls.Character.TargetLocking.performed += context => playerCharacter.SwitchTargetLockingState();

        controls.Character.Reload.performed += context => playerCharacter.Reload();

        controls.Character.EquipItem.performed += context =>
        {
            if (context.ReadValue<float>() > 0)
                playerCharacter.NextItem();
            else
                playerCharacter.PreviousItem();
        };

        controls.Character.EquipFirstSlot.performed += context => playerCharacter.EquipItemInSlot(1);
        controls.Character.EquipSecondSlot.performed += context => playerCharacter.EquipItemInSlot(2);
        controls.Character.EquipThirdSlot.performed += context => playerCharacter.EquipItemInSlot(3);
        controls.Character.EquipFourthSlot.performed += context => playerCharacter.EquipItemInSlot(4);

    }

}
