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
    [SerializeField] private EndPopupView endPopupViewPrefab;

    private bool isMainMenuOpened = false;

    [Space]
    [Header("Victory Settings")]
    private VictoryPopupModel _victoryModel = new VictoryPopupModel();
    private DefeatPopupModel _defeatModel = new DefeatPopupModel();
    private EndPopupPresenter _victoryPresenter;
    private EndPopupPresenter _defeatPresenter;

    private bool isEndPopupOpened = false;

    [Space]
    [Header("Shop Settings")]
    [SerializeField] private ShopPopupView shopPopupPrefab;
    private ShopPopupPresenter _shopPresenter;
    private ShopPopupModel _shopModel = new ShopPopupModel();

    [Space]
    [Header("Created instances / FOR DEBUG PURPOSE ONLY")]
    public GameObject activeCanvasGO;
    public MainMenu mainMenu;
    public EndPopupView endPopupView;
    public ShopPopupView shopPopupView;
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
        endPopupView = Instantiate(endPopupViewPrefab);
        shopPopupView = Instantiate(shopPopupPrefab);
        _victoryPresenter = new EndPopupPresenter(_victoryModel, endPopupView);
        _defeatPresenter = new EndPopupPresenter(_defeatModel, endPopupView);
        _shopPresenter = new ShopPopupPresenter(_shopModel, shopPopupView);

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
        AssignWavesTimerWidget();
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

        int weaponAmmo = 0;
        int totalAmmo = 0;

        // ѕровер€ем, €вл€етс€ ли оружие дальнобойным
        if (weapon is IRangedWeapon rangedWeapon)
        {
            weaponAmmo = rangedWeapon.GetAmmo();
            totalAmmo = equipComponent.GetAvaliableAmmunitionForCurrentWeapon();
        }

        equipComponent.OnCurrentWeaponAmmoChangedEvent += ammoWidget.UpdateAmmoWidget;
    }

    private void AssignWavesTimerWidget()
    {
        WavesTimerWidget wavesTimerWidget = playerHUD.waveTimerWidget;
        EnemySpawnerManager enemySpawnerManager = FindObjectOfType<EnemySpawnerManager>();

        // Assign widget
        enemySpawnerManager.OnWaveTimerChanged += wavesTimerWidget.UpdateWavesTime;
        enemySpawnerManager.OnWaveNameChanged += wavesTimerWidget.SetWaveName;
        enemySpawnerManager.OnWaveTypeChanged += wavesTimerWidget.SetWaveGoal;
        // Initilize widget
        wavesTimerWidget.UpdateWavesTime(0f);
        wavesTimerWidget.SetWaveGoal((int)enemySpawnerManager.GetCurrentWave().waveType);
        wavesTimerWidget.SetWaveName(enemySpawnerManager.GetCurrentWave().waveName);
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

    #region Main menu methods
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
    #endregion

    #region Magazine methods
    public void OpenMagazineShop()
    {
        if (isEndPopupOpened) return;
        Pause();
        _shopPresenter.ShowPopup();
        isEndPopupOpened = true;
    }

    public void CloseMagazineShop()
    {
        Unpause();
        _shopPresenter.HidePopup();
        isEndPopupOpened = false;
        GameManager.instance.OnCombatPhaseStarted?.Invoke();
    }

    #endregion

    #region Victory/Defeat methods
    public void OpenVictoryPopup()
    {
        if (isEndPopupOpened) { return; }

        Pause();
        _victoryPresenter.ShowPopup();
    }

    public void CloseVictoryPopup()
    {
        Unpause();
        _victoryPresenter.HidePopup();
    }

    public void OpenDefeatPopup()
    {
        if (isEndPopupOpened) { return; }
        _defeatPresenter.ShowPopup();
    }

    public void CloseDefeatPopup()
    {
        _defeatPresenter.HidePopup();
    }
    #endregion

    private void ToggleCanvas(GameObject canvasToToggleGO)
    {
        activeCanvasGO.SetActive(false);
        activeCanvasGO = canvasToToggleGO;
        activeCanvasGO.SetActive(true);
    }

    public void ShowWaitNextWaveGoal()
    {
        WavesTimerWidget wavesTimerWidget = playerHUD.waveTimerWidget;
        if (wavesTimerWidget != null)
        {
            //hardcode
            wavesTimerWidget.SetWaveGoal(2);
        }
    }
}
