using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
