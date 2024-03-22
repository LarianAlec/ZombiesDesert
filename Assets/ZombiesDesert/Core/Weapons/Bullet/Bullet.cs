using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject hitVFX;
    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject impactFX = Instantiate(hitVFX, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(impactFX, 1.0f);
        }

        ObjectPool.instance.ReturnBullet(gameObject);
    }
}
