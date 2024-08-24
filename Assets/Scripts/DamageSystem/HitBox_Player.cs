using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox_Player : HitBox
{
    private PlayerCharacter player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<PlayerCharacter>();
    }

    public override void TakeDamage()
    {
        base.TakeDamage();

        player.health.ReduceHealth();
    }
}
