using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // CombatPhase, PreparingPhase
    public Action OnPreparePhaseStarted;
    public Action OnCombatPhaseStarted;

    private void Start()
    {
        OnPreparePhaseStarted += PreparePhaseStart;
    }

    private void PreparePhaseStart()
    {
        Debug.Log("Prepare phase start!");
    }
}
