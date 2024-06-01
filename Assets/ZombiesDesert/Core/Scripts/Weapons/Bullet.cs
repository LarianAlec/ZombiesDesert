using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce = 1.0f; // placeholder

    [SerializeField] private GameObject hitVFX;
    [SerializeField] private AudioClip fleshImpactSound;
    [SerializeField] private AudioClip sandImpactSound;
    [SerializeField] private AudioClip metalImpactSound;
    [Range(0.0001f, 1.0f)]
    [SerializeField] private float SFXVolume = 1.0f;

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
            PlayImpactSound(collision, impactFX.transform);
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

    private void PlayImpactSound(Collision collision, Transform impact)
    {
        int collisionLayer = collision.gameObject.layer;

        int groundLayerMask = LayerMask.GetMask("Enemy");


        if (1 << collisionLayer == groundLayerMask)
        {
            if (fleshImpactSound != null)
                SoundFXManager.instance?.PlaySoundFXClip(fleshImpactSound, impact, SFXVolume/2);
        }
        else if (1 << collisionLayer == LayerMask.GetMask("Ground"))
        {
            if (sandImpactSound != null)
                SoundFXManager.instance?.PlaySoundFXClip(sandImpactSound, impact, SFXVolume);
        }
        else if (1 << collisionLayer == LayerMask.GetMask("Obstacles"))
        {
            if (metalImpactSound != null)
                SoundFXManager.instance?.PlaySoundFXClip(metalImpactSound, impact, SFXVolume);
        }

        
    }
}
