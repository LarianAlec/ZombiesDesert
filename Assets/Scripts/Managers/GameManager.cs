using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // CombatPhase, PreparingPhase
    public Action OnPreparePhaseStarted;
    public Action OnCombatPhaseStarted;

    private UI_Manager UImanager;

    private void Awake()
    {
        OnPreparePhaseStarted += PreparePhaseStart;
        OnCombatPhaseStarted += CombatPhaseStart;
    }

    private void Start()
    {
        UImanager = FindObjectOfType<UI_Manager>();
    }

    private void PreparePhaseStart()
    {
        Debug.Log("Prepare phase start!");
        UImanager.OpenVictoryPopup();
    }

    private void CombatPhaseStart()
    {
        Debug.Log("Combat phase start!");
    }

    private void OnDestroy()
    {
        OnPreparePhaseStarted -= PreparePhaseStart;
        OnCombatPhaseStarted -= CombatPhaseStart;
    }
}
