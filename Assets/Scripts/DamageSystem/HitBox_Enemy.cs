using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox_Enemy : HitBox
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        enemy.GetHit();
    }

}
