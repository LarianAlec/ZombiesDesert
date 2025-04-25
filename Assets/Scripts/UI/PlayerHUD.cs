using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    public AmmoWidget ammoWidget;
    public WavesTimerWidget waveTimerWidget;

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

}
