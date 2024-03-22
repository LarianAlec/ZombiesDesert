using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private BaseCharacter player;

    [Header("Loadout")]
    [SerializeField] private Weapon currentWeapon;

    [Space]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;


    

    private void Start()
    {
        player = GetComponent<BaseCharacter>();

        currentWeapon = weaponSlots[0];
    }

#region Getters and Setters

    public Weapon GetCurrentWeapon() => currentWeapon;

#endregion

    public void EquipWeaponInSlot(int i)
    {
        currentWeapon = weaponSlots[i];
    }
    
    public void Shoot()
    {
        if (!currentWeapon.CanShoot())
        {
            return;
        }
        //currentWeapon.ammoInMagazine--;

        /*GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(muzzle.forward));
        bullet.GetComponent<Rigidbody>().velocity = GetBulletDirection() * bulletSpeed;
        Destroy(bullet, 10);*/
        GetComponentInChildren<Animator>().SetTrigger("MakeShoot");
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("PlayerWeaponController.PickupWeapon() : No slots avaliable");
            return;
        }
            
        weaponSlots.Add(newWeapon);
    }

    public void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
            return;

        weaponSlots.Remove(currentWeapon);
        currentWeapon = weaponSlots[0];
    }

    private Vector3 GetBulletDirection()
    {
        Vector3 aimDirection = player.aim.GetAimDirection();
        return aimDirection;
    }

}
