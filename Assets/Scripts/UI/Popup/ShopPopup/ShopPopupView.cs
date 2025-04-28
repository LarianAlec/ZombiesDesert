using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopupView : MonoBehaviour, IPopupView
{
    [Header("UI Elements")]
    public Button AmmoUpgradeButton;
    public Button HealthUpgradeButton;
    public Button CloseButton;
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI CoinsText;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        gameObject.SetActive(true);
        HeaderText.text = message;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateCoins(int coins)
    {
        CoinsText.text = $"COINS: {coins}";
    }

    public void SetHeader(string header)
    {
        HeaderText.text = header;
    }
}
