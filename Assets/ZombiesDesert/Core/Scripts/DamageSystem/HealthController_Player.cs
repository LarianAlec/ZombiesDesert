using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController_Player : HealthController
{
    public delegate void FOnHealthChanged(float health, float maxHealth);
    public event FOnHealthChanged OnHealthChangedEvent;

    private PlayerCharacter player;
    public bool isDead { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerCharacter>();
    }

    public override void ReduceHealth()
    {
        base.ReduceHealth();

        if (ShouldDie())
            Die();

        OnCurrentHealthChanged(currentHealth, maxHealth);
    }

    private void Die()
    {
        isDead = true;

        Animator animator = GetComponentInChildren<Animator>();
        animator.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;

        player.ragdoll.EnableRagdoll();
    }

    private void OnCurrentHealthChanged(float health, float maxHealth)
    {
        if (OnHealthChangedEvent != null)
        {
            OnHealthChangedEvent(health, maxHealth);
        }
    }
}
