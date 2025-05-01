using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth()
    {
        currentHealth--;
    }

    public virtual void IncreaseHealth()
    {
        currentHealth++;

        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }
    }

    public virtual bool RestoreHealth(int health)
    {
        if (isCanRestoreHP())
        {
            currentHealth += health;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            return true;
        }
        return false;
    }    

    public bool isCanRestoreHP() =>  currentHealth < maxHealth; 

    public bool ShouldDie() => currentHealth <= 0;
}
