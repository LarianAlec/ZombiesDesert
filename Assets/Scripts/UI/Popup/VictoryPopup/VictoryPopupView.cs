using TMPro;
using UnityEngine;

public class VictoryPopupView : MonoBehaviour, IPopupView
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI victoryText;
    
    public void Show(string message)
    {
        victoryText.text = message;
        popupPanel.SetActive(true);
    } 
    
    public void Hide()
    {
        popupPanel.SetActive(false);
    }
}
