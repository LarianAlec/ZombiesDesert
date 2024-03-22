using System;
using System.Collections;
using UnityEngine;


[System.Serializable]
public class Weapon : MonoBehaviour
{
    public delegate void FOnReloadComplete();
    public event FOnReloadComplete OnReloadComplete;

    public delegate void FOnAmmoChanged(int newAmmo);
    public event FOnAmmoChanged OnAmmoChanged;

    public Transform muzzleSocket;
    public Transform leftHandIKSoket;
    public BaseCharacter characterOwner;

    [Space]

    [Header("Weapon attributes")]
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected WeaponFireMode weaponFireMode;
    [SerializeField] protected AmmunitionType ammoType;
    [SerializeField] protected int maxAmmo = 30;

    private int ammo = 0;
    private bool bIsReloading = false;
    private bool bIsFiring = false;

    [Space]

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [Space]

    [Header("Animations")]
    [SerializeField] private AnimationClip reloadClip;
    [SerializeField] private AnimationClip equipClip;

    private void Awake()
    {
        muzzleSocket = transform.Find(Constants.MuzzleSocket);
        SetAmmo(maxAmmo);
    }

    private void Start()
    {
        InitializeCharacterOwner();
    }

    bool IsFiring()
    {
        return bIsFiring;
    }

    public void StartReload()
    {
        if (characterOwner == null)
        {
            Debug.Log("Weapon.StartReload() : characterOwner == null!");
            return;
        }

        bIsReloading = true;
        
        characterOwner.GetAnimInstance().RunReloadAnimation();
        Action action = () => EndReload(true);
        StartCoroutine(SetReloadingToFalseAtTheEndReloadingAnimation(action));
    }

    public void EndReload(bool bIsSuccess)
    {
        if (!bIsReloading)
        {
            return;
        }

        bIsReloading = false;
        Debug.Log("Reload complete!");
        if (bIsSuccess && OnReloadComplete != null)
        {
            OnReloadComplete.Invoke();
        }
    }

    public bool IsReloading()
    {
        return bIsReloading;
    }

    public void StartFire()
    {
        MakeShot();
        if (weaponFireMode == WeaponFireMode.FullAuto)
        {
            Debug.Log("Weapon::StartFire() : Require implementation full auto fire mode");
        }
    }

    public void StopFire() 
    {
        // clear shot timer
    }

    public void MakeShot()
    {
        //Debug.Log("Weapon::MakeShot() : Reqire implement MakeShot() method");
        BaseCharacter characterOwner = GetCharacterOwner();
        if (characterOwner == null)
        {
            Debug.Log("Weapon::MakeShot() : failure, characterOwner must be valid");
            return;
        }

        if (!CanShoot())
        {
            return;
        }

        AnimatorController animInstance = characterOwner.GetAnimInstance();
        animInstance.RunShootAnimation();

        SetAmmo(ammo - 1);

        GameObject bullet = Instantiate(bulletPrefab, muzzleSocket.position, Quaternion.LookRotation(muzzleSocket.forward));
        bullet.GetComponent<Rigidbody>().velocity = GetBulletDirection() * bulletSpeed;
        Destroy(bullet, 10);
    }

    #region Utility methods
    public bool CanShoot()
    {
        return ammo > 0;
    }

    private void InitializeCharacterOwner()
    {
        if (characterOwner == null)
        {
            characterOwner = Helper.TryGetCharacterOwner(this.gameObject);
        }
    }

    public BaseCharacter GetCharacterOwner()
    {
        return characterOwner;
    }

    public void SetOwner(BaseCharacter newOwner)
    {
        characterOwner = newOwner;
    }

    public void SetAmmo(int newAmmo)
    {
        ammo = newAmmo;
        if (OnAmmoChanged != null)
        {
            OnAmmoChanged.Invoke(ammo);
        }
    }

    public int GetAmmo() => ammo;

    public int GetMaxAmmo() => maxAmmo;

    private Vector3 GetBulletDirection()
    {
        Vector3 aimDirection = characterOwner.aim.GetAimDirection();
        return aimDirection;
    }

    public AmmunitionType GetAmmoType()
    {
        return ammoType;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public AnimationClip GetCharacterEquipClip()
    {
        return equipClip;
    }

    #endregion

    #region Coroutines

    IEnumerator SetReloadingToFalseAtTheEndReloadingAnimation(Action doLast)
    {
        float reloadDuration = reloadClip.length;
        yield return new WaitForSeconds(reloadDuration);
        doLast();
    }

    #endregion
}
