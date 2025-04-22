using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Prefabs to create")]
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private MainMenu mainMenuPrefab;
    [SerializeField] private VictoryPopupView victoryPopupPrefab;

    private bool isMainMenuOpened = false;

    [Space]
    [Header("Created instances / FOR DEBUG PURPOSE ONLY")]
    public GameObject activeCanvasGO;
    public VictoryPopupView victoryPopup;
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
        mainMenu = Instantiate(mainMenuPrefab);
        victoryPopup = Instantiate(victoryPopupPrefab);


        // Set active canvas (as default it's playerHUD)
        activeCanvasGO = playerHUD.gameObject;

        // Find player character to assign purpose
        playerCharacter = FindObjectOfType<PlayerCharacter>();

        // Assign events
        StartCoroutine(AssignWidgets());
    }

    IEnumerator AssignWidgets()
    {
        yield return new WaitForEndOfFrame();
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
        Weapon weapon = equipComponent.GetCurrentEquippedWeapon();

        // Initial widget update
        int weaponAmmo = weapon.GetAmmo();
        int totalAmmo = equipComponent.GetAvaliableAmmunitionForCurrentWeapon();
        ammoWidget.UpdateAmmoWidget(weaponAmmo, totalAmmo);
        
        equipComponent.OnCurrentWeaponAmmoChangedEvent += ammoWidget.UpdateAmmoWidget;
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
        if (isMainMenuOpened)
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
        
        isMainMenuOpened = true;
    }

    public void CloseMainMenu()
    {
        Unpause();
        ToggleCanvas(playerHUD.gameObject);
        isMainMenuOpened = false;
    }

    public void OpenVictoryPopup()
    {
        Pause();
        ToggleCanvas(victoryPopup.gameObject);
        victoryPopup.Show("Победа");
    }

    public void CloseVictoryPopup()
    {
        Unpause();
        ToggleCanvas(playerHUD.gameObject);
        victoryPopup.Hide();
    }

    private void ToggleCanvas(GameObject canvasToToggleGO)
    {
        activeCanvasGO.SetActive(false);
        activeCanvasGO = canvasToToggleGO;
        activeCanvasGO.SetActive(true);
    }
}
