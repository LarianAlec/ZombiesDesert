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
    private int ammo = 0;
    private bool bIsReloading = false;
    private bool bIsFiring = false;
    private Coroutine firingModeCoroutine;

    [Space]
    [Header("Weapon attributes")]
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected AmmunitionType ammoType;
    [SerializeField] protected int maxAmmo = 30;

    [Space]
    [Header("FireMode")]
    [SerializeField] protected WeaponFireMode weaponFireMode;
    [SerializeField] protected float fireRate = 1.0f; // Bullets per second
    private float lastShootTime;

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
        bIsFiring = true;
        switch (weaponFireMode)
        {
            case WeaponFireMode.Single:
                MakeShot();
                Debug.Log("Weapon::StartFire() : Fire mode is SINGLE");
                break;

            case WeaponFireMode.FullAuto:
                firingModeCoroutine = StartCoroutine(FullAutoFiring(GetShotTimerInterval()));
                Debug.Log("Weapon::StartFire() : Fire mode is FULLAUTO");
                break;
        }
    }

    public void StopFire() 
    {
        bIsFiring = false;
        if (weaponFireMode == WeaponFireMode.FullAuto)
        {
            StopCoroutine(firingModeCoroutine);
        }
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

        GameObject bullet = ObjectPool.instance.GetBullet();

        bullet.transform.position = muzzleSocket.position;
        bullet.transform.rotation = Quaternion.LookRotation(muzzleSocket.forward);

        bullet.GetComponent<Rigidbody>().velocity = GetBulletDirection() * bulletSpeed;
    }

    #region Utility methods
    public bool CanShoot()
    {
        return ammo > 0 && ReadyToFire();
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

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1/fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }

    private float GetShotTimerInterval()
    {
        return 1.0f / fireRate;
    }

    #endregion

    #region Coroutines

    IEnumerator SetReloadingToFalseAtTheEndReloadingAnimation(Action doLast)
    {
        float reloadDuration = reloadClip.length;
        yield return new WaitForSeconds(reloadDuration);
        doLast();
    }

    IEnumerator FullAutoFiring(float shotInterval)
    {
        while (bIsFiring)
        {
            MakeShot();
            yield return new WaitForSeconds(shotInterval);
        }
    }

    #endregion
}
