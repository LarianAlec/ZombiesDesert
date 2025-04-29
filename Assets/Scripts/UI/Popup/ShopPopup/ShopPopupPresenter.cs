using System;
using UnityEngine;

public class ShopPopupPresenter : PopupPresenter
{
    private readonly ShopPopupModel _shopModel;
    private readonly ShopPopupView _shopView;

    public ShopPopupPresenter(ShopPopupModel model, ShopPopupView view) : base(model, view)
    {
        _shopModel = model;
        _shopView = view;
        InitializeShopLogic();
    }

    private void InitializeShopLogic()
    {
        _shopView.HealthUpgradeButton.onClick.AddListener(OnHealthUpgrade);
        _shopView.AmmoUpgradeButton.onClick.AddListener(OnAmmoUpgrade);
        _shopView.CloseButton.onClick.AddListener(() => UI_Manager.instance.CloseMagazineShop());
    }

    public new void ShowPopup()
    {
        base.ShowPopup();
        _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        _shopView.SetHeader("WEAPONS SHOP");
    }

    public new void HidePopup()
    {
        base.HidePopup();
    }

    private void OnHealthUpgrade()
    {
        if (_shopModel.TryBuyItem(200))
        {
            Debug.Log("GameManager.instance.playerCharacter.IncreaseMaxHealth(50);");
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }

    private void OnAmmoUpgrade()
    {
        if (_shopModel.TryBuyItem(150))
        {
            Debug.Log("GameManager.instance.playerCharacter.AddAmmo(100);");
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
}
