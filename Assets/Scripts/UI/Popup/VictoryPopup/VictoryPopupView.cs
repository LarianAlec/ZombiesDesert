using TMPro;
using UnityEngine;

public class VictoryPopupView : MonoBehaviour, IPopupView
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI victoryText;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        victoryText.text = message;
        popupPanel.SetActive(true);
    } 
    
    public void Hide()
    {
        popupPanel.SetActive(false);
    }

    public void FindUIManagerAndCloseVictoryPopup()
    {
        UI_Manager UIManager = FindObjectOfType<UI_Manager>();
        UIManager?.CloseVictoryPopup();

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager?.OnCombatPhaseStarted?.Invoke();
    }
}
