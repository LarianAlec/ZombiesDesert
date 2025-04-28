using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // CombatPhase, PreparingPhase
    public Action OnPreparePhaseStarted;
    public Action OnMagazinePhaseStarted;
    public Action OnCombatPhaseStarted;
    public Action OnPlayerCharacterDied;

    private UI_Manager UImanager;

    private PlayerCharacter playerCharacter;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        OnPreparePhaseStarted += PreparePhaseStart;
        OnCombatPhaseStarted += CombatPhaseStart;
        OnPlayerCharacterDied += PlayerCharacterDied;
        OnMagazinePhaseStarted += MagazinePhaseStart;
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

    private void PlayerCharacterDied()
    {
        Debug.Log("PlayerCharacter died");
        UImanager.OpenDefeatPopup();
        OnPlayerCharacterDied -= PlayerCharacterDied;
    }

    private void MagazinePhaseStart()
    {
        Debug.Log("Magazine phase start!");
        UImanager.OpenMagazineShop();
    }

    private void OnDestroy()
    {
        OnPreparePhaseStarted -= PreparePhaseStart;
        OnCombatPhaseStarted -= CombatPhaseStart;
        OnPlayerCharacterDied -= PlayerCharacterDied;
        OnMagazinePhaseStarted -= MagazinePhaseStart;
    }
}
