using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class WavesTimerWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainTimerText;
    [SerializeField] private TextMeshProUGUI waveNameText;
    [SerializeField] private TextMeshProUGUI waveGoalText;

    public enum WaveGoal
    {
        Survive,
        Buy, 
        WaitNextWave
    }

    public void UpdateWavesTime(float currentTime)
    {
        if (currentTime >= 0.1f)
            remainTimerText.SetText(currentTime.ToString("0.0"));
        else 
            remainTimerText.SetText("");
    }
    
    public void SetWaveName(string text)
    {
        waveNameText.SetText(text);
    }

    public void SetWaveGoal(int goalIndex)
    {
        WaveGoal goal = (WaveGoal)goalIndex;
        string text;
        switch (goal)
        {
            case WaveGoal.Survive:
                text = "Выживи!";
                break;
            
            case WaveGoal.Buy:
                text = "Подготовься к бою!";
                break;

            case WaveGoal.WaitNextWave:
                text = "До следующей волны осталось:";
                break;
            
            default:
                text = "{WaveGoal}";
                break;
        }
        waveGoalText.SetText(text);
    }
}
