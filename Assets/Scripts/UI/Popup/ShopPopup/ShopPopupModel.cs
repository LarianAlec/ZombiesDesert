public class ShopPopupModel : PopupModel
{
    public int PlayerCoins { get; private set; } = 500; // Example of started currency

    public bool TryBuyItem(int itemCost)
    {
        if (PlayerCoins >= itemCost)
        {
            PlayerCoins -= itemCost;
            return true;
        }
        return false;
    }
}
