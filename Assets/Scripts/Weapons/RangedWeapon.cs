using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class RangedWeapon : Weapon, IRangedWeapon
{
    public event Action OnReloadComplete;
    public event Action<int> OnAmmoChanged;

    [Header("Ranged Weapon Settings")]
    [SerializeField] private WeaponType weaponType;
    [SerializeField] public Transform muzzleSocket;
    [SerializeField] private AmmunitionType ammoType;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private WeaponFireMode weaponFireMode;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float spreadAmount = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip emptyAmmoSound;
    [SerializeField] private AnimationClip reloadClip;


    private int ammo;
    private bool isReloading;
    private bool isFiring;
    private Coroutine firingCoroutine;
    private float lastShotTime;

    public int GetAmmo() => ammo;

    public int GetMaxAmmo() => maxAmmo;

    public void SetAmmo(int value)
    {
        ammo = Mathf.Clamp(value, 0, maxAmmo);
        OnAmmoChanged?.Invoke(ammo);
    }

    private void Awake()
    {
        SetAmmo(maxAmmo);
    }

    public override void StartAction()
    {
        if (isFiring || !CanShoot()) return;
        isFiring = true;

        switch (weaponFireMode)
        {
            case WeaponFireMode.Single:
                MakeShot();
                break;
            case WeaponFireMode.FullAuto:
                firingCoroutine = StartCoroutine(FullAutoFire());
                break;
        }
    }

    public override void StopAction()
    {
        isFiring = false;
        if (firingCoroutine != null)
            StopCoroutine(firingCoroutine);
    }

    public void StartFire() => StartAction();

    public void StopFire() => StopAction();

    public void EndReload(bool isSuccess)
    {
        if (!isReloading) return;
        isReloading = false;
        if (isSuccess) OnReloadComplete?.Invoke();
    }

    private IEnumerator FullAutoFire()
    {
        while (isFiring)
        {
            MakeShot();
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    private void MakeShot()
    {
        if (!CanShoot())
        {
            if (ammo == 0)
                SoundFXManager.instance?.PlaySoundFXClip(emptyAmmoSound, transform, 1f);
            return;
        }

        SoundFXManager.instance?.PlaySoundFXClip(fireSound, transform, 1f);
        characterOwner.GetAnimInstance().RunShootAnimation();
        SetAmmo(ammo - 1);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            var bullet = ObjectPool.instance.GetBullet();
            bullet.transform.SetPositionAndRotation(muzzleSocket.position, muzzleSocket.rotation);

            Vector3 direction = GetBulletDirection();
            direction = ApplySpread(direction);

            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
        }
    }

    public void StartReload()
    {
        if (isReloading || ammo >= maxAmmo) return;
        isReloading = true;
        characterOwner.GetAnimInstance().RunReloadAnimation();
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadClip.length);
        SetAmmo(maxAmmo);
        isReloading = false;
        OnReloadComplete?.Invoke();
    }

    private Vector3 GetBulletDirection() => characterOwner.aim.GetAimDirection();

    private Vector3 ApplySpread(Vector3 dir) => Quaternion.Euler(
        UnityEngine.Random.Range(-spreadAmount, spreadAmount),
        UnityEngine.Random.Range(-spreadAmount, spreadAmount),
        0) * dir;

    public bool CanShoot() => ammo > 0 && Time.time > lastShotTime + 1f / fireRate;

    public override WeaponType GetWeaponType() => weaponType;

    public AmmunitionType GetAmmoType() => ammoType;
    public bool IsReloading() => isReloading;
}
