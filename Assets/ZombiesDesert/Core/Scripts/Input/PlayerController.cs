using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Player player;

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
        player = GetComponent<Player>();

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

        controls.Character.Fire.performed += context => player.Fire();

        controls.Character.EquipFirstSlot.performed += context => player.EquipFirstSlot();
        controls.Character.EquipSecondSlot.performed += context => player.EquipSecondSlot();
        controls.Character.EquipThirdSlot.performed += context => player.EquipThirdSlot();
        controls.Character.EquipFourthSlot.performed += context => player.EquipFourthSlot();
    }
}
