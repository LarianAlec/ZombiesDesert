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
}
