using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();        
    }

    public void ExitCurrentAIState() => enemy.ExitCurrentAIStateViaAnimEvent();

    public void StartManualMovement() => enemy.SetManualMovement(true);

    public void StopManualMovement() => enemy.SetManualMovement(false);

    public void StartManualRotation() => enemy.SetManualRotation(true);

    public void StopManualRotation() => enemy.SetManualRotation(false);
    
}
