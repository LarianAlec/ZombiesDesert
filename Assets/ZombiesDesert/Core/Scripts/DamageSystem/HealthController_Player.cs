using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController_Player : HealthController
{
    private PlayerCharacter player;
    private PlayerHUD playerHUD;
    public bool isDead { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerCharacter>();
    }

    private void Start()
    {
        playerHUD = FindObjectOfType<UI_Manager>().playerHUD;
    }

    public override void ReduceHealth()
    {
        base.ReduceHealth();

        if (ShouldDie())
            Die();

        playerHUD.UpdateHealthUI(currentHealth, maxHealth);
    }


    private void Die()
    {
        isDead = true;

        Animator animator = GetComponentInChildren<Animator>();
        animator.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;

        player.ragdoll.EnableRagdoll();
    }
}
