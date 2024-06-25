
using UnityEngine;

public interface IEnemy
{
    void GetHit();
    void GetHitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb);
    void Die();
    void EnterBattleMode();
}
