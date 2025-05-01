using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    [SerializeField] private int playerCoins;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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
        CharacterEquipmentComponent equipmentComponent = GameManager.instance.GetPlayer().GetComponent<CharacterEquipmentComponent>();
        equipmentComponent.AddAmmo(AmmunitionType.Pistol, 30);
    }
    public void AddShotgunAmmo()
    {
        CharacterEquipmentComponent equipmentComponent = GameManager.instance.GetPlayer().GetComponent<CharacterEquipmentComponent>();
        equipmentComponent.AddAmmo(AmmunitionType.Shotgun, 8);
    }

    public void AddRifleAmmo()
    {
        CharacterEquipmentComponent equipmentComponent = GameManager.instance.GetPlayer().GetComponent<CharacterEquipmentComponent>();
        equipmentComponent.AddAmmo(AmmunitionType.AutoRifle, 30);
    }
}
