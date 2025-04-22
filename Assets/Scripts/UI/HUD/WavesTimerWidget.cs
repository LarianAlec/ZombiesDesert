using UnityEngine;
using TMPro;

public class WavesTimerWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    public void UpdateWavesTime(float currentTime)
    {
        timerText.SetText(currentTime.ToString("0.0"));
    }
}
