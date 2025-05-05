using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    [SerializeField] private int playerCoins;
    private CharacterEquipmentComponent playerEquipmentComponent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerEquipmentComponent = GameManager.instance.GetPlayer().GetComponent<CharacterEquipmentComponent>();
    }

    public bool TrySpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            return true;
        }
        return false;
    }
    
    public void AddCoins(int amount)
    {
        playerCoins += amount;
    }
    
    public int GetPlayerCoins()
    {
        return playerCoins;
    }

    public bool RestorePlayerHealth(int health)
    {
        HealthController_Player playerHealth = GameManager.instance.GetPlayer().GetComponent<HealthController_Player>();
        return playerHealth.RestoreHealth(health);
    }

    public void AddPistolAmmo()
    {
        playerEquipmentComponent.AddAmmo(AmmunitionType.Pistol, 30);
    }
    public void AddShotgunAmmo()
    {
        playerEquipmentComponent.AddAmmo(AmmunitionType.Shotgun, 8);
    }

    public void AddRifleAmmo()
    {
        playerEquipmentComponent.AddAmmo(AmmunitionType.AutoRifle, 30);
    }

    public void UpgradePistol()
    {
        Debug.Log("UpgradePistol");
        playerEquipmentComponent.UpgradeWeapon(WeaponType.Pistol);
    }

    public void UpgradeShotgun()
    {
        Debug.Log("UpgradeShotgun");
        playerEquipmentComponent.UpgradeWeapon(WeaponType.Shotgun);
    }

    public void UpgradeRifle()
    {
        Debug.Log("UpgradeRifle");
        playerEquipmentComponent.UpgradeWeapon(WeaponType.AutoRifle);
    }
}
