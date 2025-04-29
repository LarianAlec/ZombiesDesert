using UnityEngine;
public class ShopPopupModel : PopupModel
{
    public bool TryBuyItem(int itemCost)
    {
        return ShopManager.instance.TrySpendCoins(itemCost);
    }

    public int GetPlayerCoins()
    {
        return ShopManager.instance.GetPlayerCoins();
    }
}
