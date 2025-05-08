using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Weapon Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private AudioClip swingSound;
    [SerializeField] private AnimationClip attackClip;

    private bool isAttacking;
    private float lastAttackTime;

    public override void StartAction()
    {
        if (isAttacking || !CanAttack()) return;

        StartCoroutine(AttackRoutine());
    }

    public override void StopAction()
    {

    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        SoundFXManager.instance?.PlaySoundFXClip(swingSound, transform, 1f);
        characterOwner.GetAnimInstance().PlayAnimation(attackClip);

        yield return new WaitForSeconds(attackClip.length * 0.3f);
        DetectHits();

        yield return new WaitForSeconds(attackClip.length * 0.7f);
        isAttacking = false;
    }

    private void DetectHits()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            0.5f,
            characterOwner.transform.forward,
            attackRange
        );

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    private bool CanAttack() => Time.time > lastAttackTime + attackCooldown;
    public override WeaponType GetWeaponType() => WeaponType.Melee;
}

