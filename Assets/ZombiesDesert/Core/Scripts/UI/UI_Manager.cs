using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Prefabs to create")]
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private MainMenu menuPrefab;

    private bool isMenuOpened= false;

    [Space]
    [Header("Pause components to deactivate")]
    [SerializeField] private TopDownAimComponent topDownAimComponent;
    [SerializeField] private CharacterEquipmentComponent characterEquipmentComponent;
    [SerializeField] private PlayerController playerController;
    MonoBehaviour[] components;

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

        // Components to deactivate when paused
        components = new MonoBehaviour[3] { topDownAimComponent, characterEquipmentComponent, playerController };
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
        Time.timeScale = 0.0f;
        foreach (MonoBehaviour component in components)
        {
            component.enabled = false;
        }

        
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
        foreach (MonoBehaviour component in components)
        {
            component.enabled = true;
        }
    }

    #endregion

    #region Main Menu Activations/Deactivations

    public void OpenCloseMenu()
    {
        if (isMenuOpened)
        {
            // Close menu
            UntoggleMainMenu();
        }
        else
        {
            // Open menu
            ToggleMainMenu();
        }
    }

    #endregion

    public void ToggleMainMenu()
    {
        Pause();
        ToggleCanvas(mainMenu.gameObject);
        isMenuOpened = true;
    }

    public void UntoggleMainMenu()
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
