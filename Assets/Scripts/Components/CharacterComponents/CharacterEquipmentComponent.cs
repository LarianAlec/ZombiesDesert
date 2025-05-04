using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterEquipmentComponent : MonoBehaviour
{
    public delegate void FOnCurrentWeaponAmmoChanged(int ammo, int avaliableAmmunitionForCurrentWeapon);
    public event FOnCurrentWeaponAmmoChanged OnCurrentWeaponAmmoChangedEvent;

    [System.Serializable]
    public class AmmunitionAmount
    {
        public AmmunitionType ammunitionType;
        public int amount;
    }

    [System.Serializable]
    public class WeaponsArray
    {
        public WeaponType weaponType;
        public EquipmentSlots equipmentSlot;
        public GameObject gunPrefab;
        [Tooltip("Создавать ли это оружие при старте игры?")]
        public bool createOnStart = true;
    }

    private bool bIsEquipping = false;

    [Header("Weapons")]
    [SerializeField] private Transform weaponSocket;
    [SerializeField] protected AmmunitionAmount[] startAmmunition;
    [SerializeField] protected WeaponsArray[] startWeapons;
    [SerializeField] protected EquipmentSlots autoEquipSlot;

    [Header("Upgrades")]
    [SerializeField] private UpgradedWeapon[] upgradedWeapons;

    public Weapon currentEquippedWeapon;
    public EquipmentSlots currentEquippedSlot;

    private Transform holderSocket;
    private BaseCharacter cachedCharacter;
    private PlayerWeaponController weaponController;
    private Dictionary<AmmunitionType, int> ammunitionArray;
    private Dictionary<EquipmentSlots, Weapon> weaponsArray;


    private void Awake()
    {
        holderSocket = transform.Find(Constants.WeaponHolderSocket);
    }

    void Start()
    {
        cachedCharacter = GetComponent<BaseCharacter>();
        CreateLoadout();
        AutoEquip();
    }

    public WeaponType GetCurrentEquippedWeaponType()
    {
        WeaponType result = WeaponType.None;
        if (currentEquippedWeapon != null)
        {
            result = currentEquippedWeapon.GetWeaponType();
        }
        return result;
    }

    public Weapon GetCurrentEquippedWeapon()
    {
        return currentEquippedWeapon;
    }

    public bool IsEquipping() => bIsEquipping;

    public void ReloadCurrentWeapon()
    {
        if (currentEquippedWeapon == null)
        {
            return;
        }
        int avaliableAmmunition = GetAvaliableAmmunitionForCurrentWeapon();
        if (avaliableAmmunition <= 0)
        {
            return;
        }

        currentEquippedWeapon.StartReload();
    }

    public void EquipItemInSlot(EquipmentSlots slot)
    {
        if (bIsEquipping) return;

        if (!weaponsArray.ContainsKey(slot) || weaponsArray[slot] == null)
        {
            Debug.LogWarning($"Slot {slot} is empty!");
            return;
        }

        UnEquipCurrentItem();
        currentEquippedWeapon = weaponsArray[slot];

        if (currentEquippedWeapon != null)
        {
            /*Animator animator = cachedCharacter.GetAnimInstance().animator;
            int currentAnimatorLayerIndex = cachedCharacter.GetAnimInstance().GetCurrentAnimationLayerIndex();
            animator.Play("Equip", 1);
            /*AnimationClip equipClip = currentEquippedWeapon.GetCharacterEquipClip();
            if (equipClip)
            {
                bIsEquipping = true;
                float equipDuration = 1.0f;
                SetEquippingBoolToFalse(equipDuration);
            }
            else
            {
                AttachCurrentWeaponToEquippedSocket();
            }*/
            AttachCurrentWeaponToEquippedSocket();
            currentEquippedSlot = slot;
        }

        if (currentEquippedWeapon != null)
        {
            currentEquippedWeapon.OnAmmoChanged += OnCurrentWeaponAmmoChanged;
            currentEquippedWeapon.OnReloadComplete += OnWeaponReloadComplete_Event;
            OnCurrentWeaponAmmoChanged(currentEquippedWeapon.GetAmmo());
        }
    }

    public void AttachCurrentWeaponToEquippedSocket()
    {
        currentEquippedWeapon.gameObject.SetActive(true);
    }

    public void UnEquipCurrentItem()
    {
        if (currentEquippedWeapon)
        {
            currentEquippedWeapon.StopFire();
            currentEquippedWeapon.EndReload(false);
            currentEquippedWeapon.OnAmmoChanged -= OnCurrentWeaponAmmoChanged;
            currentEquippedWeapon.OnReloadComplete -= OnWeaponReloadComplete_Event;
        }

        if (currentEquippedWeapon)
        {
            currentEquippedWeapon.gameObject.SetActive(false);
        }
    }

    public void EquipNextItem()
    {
        var validSlots = weaponsArray
       .Where(kvp => kvp.Value != null)
       .Select(kvp => kvp.Key)
       .ToList();

        if (validSlots.Count == 0) return;

        int currentIndex = validSlots.IndexOf(currentEquippedSlot);
        int nextIndex = (currentIndex + 1) % validSlots.Count;
        EquipItemInSlot(validSlots[nextIndex]);
    }

    public void EquipPreviousItem()
    {
        if (bIsEquipping) return;

        // Получаем все слоты с оружием
        var validSlots = weaponsArray
            .Where(kvp => kvp.Value != null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (validSlots.Count == 0) return;

        // Находим текущий индекс в списке
        int currentIndex = validSlots.IndexOf(currentEquippedSlot);
        if (currentIndex == -1) currentIndex = 0;

        // Вычисляем предыдущий индекс
        int previousIndex = (currentIndex - 1 + validSlots.Count) % validSlots.Count;
        EquipmentSlots previousSlot = validSlots[previousIndex];

        EquipItemInSlot(previousSlot);
    }

    private int NextWeaponArraySlotIndex(int currentSlotIndex)
    {
        if (currentSlotIndex == weaponsArray.Count)
        {
            return (int)weaponsArray.First().Key;
        }
        else
        {
            return currentSlotIndex + 1;
        }
    }

    private int PreviousWeaponArraySlotIndex(int currentSlotIndex)
    {
        if (currentSlotIndex == 1)
        {
            return weaponsArray.Count;
        }
        else
        {
            return currentSlotIndex - 1;
        }
    }

    private void AutoEquip()
    {
        if (autoEquipSlot != EquipmentSlots.None
            && weaponsArray.ContainsKey(autoEquipSlot)
            && weaponsArray[autoEquipSlot] != null)
        {
            EquipItemInSlot(autoEquipSlot);
        }
    }

    public void CreateLoadout()
    {
        ammunitionArray = new Dictionary<AmmunitionType, int>();
        foreach (AmmunitionAmount ammoPair in startAmmunition)
        {
            ammunitionArray.Add(ammoPair.ammunitionType, ammoPair.amount);
        }

        weaponsArray = new Dictionary<EquipmentSlots, Weapon>();
        foreach (WeaponsArray weaponPair in startWeapons)
        {
            if (weaponPair.createOnStart && weaponPair.gunPrefab != null)
            {
                GameObject gunObj = Instantiate(weaponPair.gunPrefab, holderSocket, false);
                Weapon gunComponent = gunObj.GetComponent<Weapon>();
                weaponsArray.Add(weaponPair.equipmentSlot, gunComponent);
                gunObj.SetActive(false);
            }
        }
    }

    public int GetAvaliableAmmunitionForCurrentWeapon()
    {
        if (currentEquippedWeapon == null)
        {
            return 0;
        }
        return ammunitionArray[GetCurrentEquippedWeapon().GetAmmoType()];
    }

    private void OnWeaponReloadComplete_Event()
    {
        int avaliableAmmunition = GetAvaliableAmmunitionForCurrentWeapon();
        int currentAmmo = currentEquippedWeapon.GetAmmo();
        int ammoToReload = currentEquippedWeapon.GetMaxAmmo() - currentAmmo;
        int reloadedAmmo = Math.Min(avaliableAmmunition, ammoToReload);

        ammunitionArray[currentEquippedWeapon.GetAmmoType()] -= reloadedAmmo;
        currentEquippedWeapon.SetAmmo(reloadedAmmo + currentAmmo);
    }

    private void OnCurrentWeaponAmmoChanged(int ammo)
    {
        if (OnCurrentWeaponAmmoChangedEvent != null)
        {
            OnCurrentWeaponAmmoChangedEvent(ammo, GetAvaliableAmmunitionForCurrentWeapon());
        }
    }

    public void AddAmmo(AmmunitionType ammoType, int amount)
    {
        ammunitionArray[ammoType] += amount;
        OnCurrentWeaponAmmoChanged(currentEquippedWeapon.GetAmmo());
    }

    public void UpgradeWeapon(WeaponType weaponType)
    {
        // Проверяем существующие слоты с оружием
        var existingSlots = weaponsArray
            .Where(kvp => kvp.Value != null && kvp.Value.GetWeaponType() == weaponType)
            .ToList();

        // Если оружия нет, создаем базовую версию FirstTier
        if (existingSlots.Count == 0)
        {
            CreateBaseWeaponIfNeeded(weaponType);
            existingSlots = weaponsArray
                .Where(kvp => kvp.Value != null && kvp.Value.GetWeaponType() == weaponType)
                .ToList();

            // Если создание не удалось - выходим
            if (existingSlots.Count == 0)
            {
                Debug.LogError($"Failed to create base {weaponType}");
                return;
            }

            // Обновляем интерфейс для нового оружия и выходим без апгрейда
            var newWeapon = existingSlots.First().Value;
            if (currentEquippedSlot == existingSlots.First().Key)
            {
                currentEquippedWeapon = newWeapon;
                OnCurrentWeaponAmmoChanged(newWeapon.GetAmmo());
            }
            return;
        }

        // Логика апгрейда существующих экземпляров
        var possibleUpgrades = upgradedWeapons
            .Where(u => u.weaponType == weaponType)
            .OrderBy(u => u.targetTier)
            .ToList();

        foreach (var slot in existingSlots)
        {
            EquipmentSlots eqSlot = slot.Key;
            Weapon oldWeapon = slot.Value;

            // Находим следующий доступный тир (строго выше текущего)
            var nextTier = possibleUpgrades.FirstOrDefault(u =>
                (int)u.targetTier > (int)oldWeapon.weaponTier);

            if (nextTier == null) continue;

            // Замена оружия
            Destroy(oldWeapon.gameObject);
            GameObject newGun = Instantiate(nextTier.upgradedPrefab, holderSocket, false);
            Weapon newWeapon = newGun.GetComponent<Weapon>();
            newWeapon.weaponTier = nextTier.targetTier;
            weaponsArray[eqSlot] = newWeapon;
            newGun.SetActive(false);

            // Обновляем текущее оружие если нужно
            if (currentEquippedSlot == eqSlot)
            {
                UnEquipCurrentItem();
                currentEquippedWeapon = newWeapon;
                currentEquippedWeapon.gameObject.SetActive(true);
                currentEquippedWeapon.OnAmmoChanged += OnCurrentWeaponAmmoChanged;
                currentEquippedWeapon.OnReloadComplete += OnWeaponReloadComplete_Event;
                OnCurrentWeaponAmmoChanged(currentEquippedWeapon.GetAmmo());
            }
        }
    }

    private void CreateBaseWeaponIfNeeded(WeaponType weaponType)
    {
        WeaponsArray startConfig = startWeapons.FirstOrDefault(w =>
            w.weaponType == weaponType);

        GameObject basePrefab = null;
        EquipmentSlots slot = EquipmentSlots.None;
        WeaponTier targetTier = WeaponTier.FirstTier;

        // Если оружие не создавалось при старте
        if (startConfig != null && !startConfig.createOnStart)
        {
            UpgradedWeapon upgradedConfig = upgradedWeapons.FirstOrDefault(u =>
                u.weaponType == weaponType && u.targetTier == WeaponTier.FirstTier);

            if (upgradedConfig != null)
            {
                basePrefab = upgradedConfig.upgradedPrefab;
                slot = startConfig.equipmentSlot;
            }
            else
            {
                Debug.LogError($"Missing FirstTier upgrade config for {weaponType}");
                return;
            }
        }
        else
        {
            // Стандартное создание из startWeapons
            startConfig = startWeapons.FirstOrDefault(w =>
                w.weaponType == weaponType && w.gunPrefab != null);

            if (startConfig == null)
            {
                Debug.LogError($"No base config for {weaponType}");
                return;
            }
            basePrefab = startConfig.gunPrefab;
            slot = startConfig.equipmentSlot;
        }

        // Создаем оружие если слот пуст
        if (!weaponsArray.ContainsKey(slot) || weaponsArray[slot] == null)
        {
            GameObject gunObj = Instantiate(basePrefab, holderSocket, false);
            Weapon weapon = gunObj.GetComponent<Weapon>();
            weapon.weaponTier = targetTier;
            weaponsArray[slot] = weapon;
            gunObj.SetActive(false);
            Debug.Log($"Created base {weaponType} (Tier {targetTier}) in slot {slot}");
        }
    }
}
