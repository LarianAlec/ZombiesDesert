using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
        }

        playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered += OnPlayerInputActionTriggered;
    }

    private void OnPlayerInputActionTriggered(InputAction.CallbackContext context)
    {
        InputAction action = context.action;

        switch (action.name)
        {
            case "MenuOpenClose":
                UI_Manager.instance.OpenCloseMenu();
                break;
        }
    }

}
