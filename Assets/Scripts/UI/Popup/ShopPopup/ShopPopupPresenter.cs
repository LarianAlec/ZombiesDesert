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
        _shopView.HealthRestoreButton.onClick.AddListener(OnHealthRestore);
        _shopView.PistolAmmoButton.onClick.AddListener(OnAddPistolAmmo);
        _shopView.ShotgunAmmoButton.onClick.AddListener(OnAddShotgunAmmo);
        _shopView.RifleAmmoButton.onClick.AddListener(OnAddRifleAmmo);
        _shopView.CloseButton.onClick.AddListener(() => UI_Manager.instance.CloseMagazineShop());
        _shopView.PistolUpgradeButton.onClick.AddListener(OnPistolUpgrade);
        _shopView.ShotgunUpgradeButton.onClick.AddListener(OnShotgunUpgrade);
        _shopView.RifleUpgradeButton.onClick.AddListener(OnRifleUpgrade);
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

    private void OnHealthRestore()
    {
        int cost = 200;
        if (_shopModel.TryBuyItem(cost))
        {
            if (ShopManager.instance.RestorePlayerHealth(25))
            {
                _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
            }
            else
            {
                ShopManager.instance.AddCoins(cost);
            }
        }
    }

    private void OnAddPistolAmmo()
    {
        if (_shopModel.TryBuyItem(150))
        {
            ShopManager.instance.AddPistolAmmo();
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
    
    private void OnAddShotgunAmmo()
    {
        if (_shopModel.TryBuyItem(150))
        {
            ShopManager.instance.AddShotgunAmmo();
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
    
    private void OnAddRifleAmmo()
    {
        if (_shopModel.TryBuyItem(150))
        {
            ShopManager.instance.AddRifleAmmo();
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
    
    private void OnPistolUpgrade()
    {
        if (_shopModel.TryBuyItem(150))
        {
            Debug.Log("OnPistolUpgrade");
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }

    private void OnShotgunUpgrade()
    {
        if (_shopModel.TryBuyItem(150))
        {
            Debug.Log("OnShotgunUpgrade");
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
    
    private void OnRifleUpgrade()
    {
        if (_shopModel.TryBuyItem(150))
        {
            Debug.Log("OnRifleUpgrade");
            _shopView.UpdateCoins(_shopModel.GetPlayerCoins());
        }
    }
}
