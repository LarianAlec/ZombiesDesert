using UnityEngine;
public class ShopPopupModel : PopupModel
{
    [SerializeField] private ShopManager shop;

    private void Start()
    {
        shop = ShopManager.instance;
    }

    public bool TryBuyItem(int itemCost)
    {
        return shop.TrySpendCoins(itemCost);
    }

    public int GetPlayerCoins()
    {
        return shop.GetPlayerCoins();
    }
}
