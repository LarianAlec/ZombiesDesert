public interface IRangedWeapon
{
    event System.Action OnReloadComplete;
    event System.Action<int> OnAmmoChanged;
    int GetAmmo();
    int GetMaxAmmo();
    void SetAmmo(int value);
    void StartReload();
    void StopFire();
    void EndReload(bool isSuccess);
    AmmunitionType GetAmmoType();
    bool CanShoot();
    void StartFire();
    bool IsReloading();
}
