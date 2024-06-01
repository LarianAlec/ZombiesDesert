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
    [Header("Spread")]
    [SerializeField] protected float spreadAmount = 0.5f;

    [Space]
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [Space]
    [Header("Animations")]
    [SerializeField] private AnimationClip reloadClip;
    [SerializeField] private AnimationClip equipClip;

    [Space]
    [Header("SFX")]
    [SerializeField] private AudioClip fireSoundClip;

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

        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(fireSoundClip, gameObject.transform, 1f);
        }

        SetAmmo(ammo - 1);

        GameObject bullet = ObjectPool.instance.GetBullet();

        if (bullet == null)
        {
            Debug.Log("Weapon::MakeShot() : Need object pool instance for bullets!");
        }

        bullet.transform.position = muzzleSocket.position;
        bullet.transform.rotation = Quaternion.LookRotation(muzzleSocket.forward);

        Vector3 shotDirection = GetBulletDirection();
        shotDirection = GetBulletSpreadOffset(shotDirection);

        bullet.GetComponent<Rigidbody>().velocity = shotDirection * bulletSpeed;
    }

    #region Utility methods

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public AmmunitionType GetAmmoType()
    {
        return ammoType;
    }

    public bool CanShoot()
    {
        return ammo > 0 && ReadyToFire();
    }

    public AnimationClip GetCharacterEquipClip()
    {
        return equipClip;
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

    private void InitializeCharacterOwner()
    {
        if (characterOwner == null)
        {
            characterOwner = Helper.TryGetCharacterOwner(this.gameObject);
        }
    }

    private float GetShotTimerInterval()
    {
        return 1.0f / fireRate;
    }

    private Vector3 GetBulletDirection()
    {
        Vector3 aimDirection = characterOwner.aim.GetAimDirection();
        return aimDirection;
    }

    private Vector3 GetBulletSpreadOffset(Vector3 originalDirection)
    {
        float randomizedValue = UnityEngine.Random.Range(-spreadAmount, spreadAmount);
        Quaternion spreadRoatation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRoatation * originalDirection;
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
