using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce = 1.0f; // placeholder

    [SerializeField] private GameObject hitVFX;
    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage();
        ApplyBulletImpactToEnemy(collision);

        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject impactFX = Instantiate(hitVFX, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(impactFX, 1.0f);
        }

        ObjectPool.instance.ReturnBullet(gameObject);
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Vector3 hitPoint = collision.contacts[0].point;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.GetHitImpact(force, hitPoint, hitRigidbody);
        }
    }
}
