using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Prefabs to create")]
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private MainMenu menuPrefab;

    private bool isMenuOpened= false;

    [Space]
    [Header("Created instances / FOR DEBUG PURPOSE ONLY")]
    public GameObject activeCanvasGO;

    public MainMenu mainMenu;
    public PlayerHUD playerHUD;
    public PlayerCharacter playerCharacter;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Creating canvases
        playerHUD = Instantiate(playerHUDPrefab);
        mainMenu = Instantiate(menuPrefab);

        // Set active canvas (as default it's playerHUD)
        activeCanvasGO = playerHUD.gameObject;

        // Find player character to assign purpose
        playerCharacter = FindObjectOfType<PlayerCharacter>();

        // Assign events
        AssignAmmoWidget();
        AssignHealthWidget();

    }


    #region Assign events

    private void AssignHealthWidget()
    {
        HealthController_Player healthController = playerCharacter.GetComponent<HealthController_Player>();
        healthController.OnHealthChangedEvent += playerHUD.UpdateHealthUI;

        // Initial widget update
        playerHUD.UpdateHealthUI(healthController.currentHealth, healthController.maxHealth);
    }

    private void AssignAmmoWidget()
    {
        CharacterEquipmentComponent equipComponent = playerCharacter.GetComponent<CharacterEquipmentComponent>();
        AmmoWidget ammoWidget = playerHUD.ammoWidget;
        equipComponent.OnCurrentWeaponAmmoChangedEvent += ammoWidget.UpdateAmmoWidget;


        // Initial widget update
        int weaponAmmo = equipComponent.GetCurrentEquippedWeapon().GetAmmo();
        int totalAmmo = equipComponent.GetAvaliableAmmunitionForCurrentWeapon();
        ammoWidget.UpdateAmmoWidget(weaponAmmo, totalAmmo);
    }

    #endregion

    #region Pause/Unpause Functions

    public void Pause()
    {
        PauseManager.instance.Pause();
    }

    public void Unpause()
    {
        PauseManager.instance.Unpause();
    }

    #endregion

    #region Main Menu Activations/Deactivations

    public void OpenCloseMenu()
    {
        if (isMenuOpened)
        {
            // Close menu
            CloseMainMenu();
        }
        else
        {
            // Open menu
            OpenMainMenu();
        }
    }

    #endregion

    public void OpenMainMenu()
    {
        Pause();

        ToggleCanvas(mainMenu.gameObject);
        mainMenu.OpenMainMenu();
        
        isMenuOpened = true;
    }

    public void CloseMainMenu()
    {
        Unpause();
        ToggleCanvas(playerHUD.gameObject);
        isMenuOpened = false;
    }

    private void ToggleCanvas(GameObject canvasToToggleGO)
    {
        activeCanvasGO.SetActive(false);
        activeCanvasGO = canvasToToggleGO;
        activeCanvasGO.SetActive(true);
    }
}
