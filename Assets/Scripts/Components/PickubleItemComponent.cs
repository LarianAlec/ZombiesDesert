using UnityEngine;

public class PickupableItemComponent : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponController>()?.PickupWeapon(weapon);
    }
}
