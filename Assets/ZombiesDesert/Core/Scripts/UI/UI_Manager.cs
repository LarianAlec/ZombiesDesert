using System;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Prefabs to create")]
    [SerializeField] private Canvas canvasPrefab;

    [Space]
    [Header("Created instances")]
    public PlayerHUD playerHUD;
    public Canvas canvas;
    public PlayerCharacter playerCharacter;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvas = Instantiate(canvasPrefab);
        playerHUD = canvas.GetComponentInChildren<PlayerHUD>();
        playerCharacter = FindObjectOfType<PlayerCharacter>();

        
        AssignAmmoWidget();
        AssignHealthWidget();
    }

    private void AssignHealthWidget()
    {
        HealthController_Player healthController = playerCharacter .GetComponent<HealthController_Player>();
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
}
