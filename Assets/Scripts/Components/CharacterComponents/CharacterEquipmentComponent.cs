using Synty.AnimationBaseLocomotion.Samples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        [Tooltip("Should create weapon on start?")]
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

    private SamplePlayerAnimationController _animationController;

    private void Awake()
    {
        holderSocket = transform.Find(Constants.WeaponHolderSocket);
        if (holderSocket == null ) 
        {
            Debug.LogWarning($"CharacterEquipmentComponent: holderSocket is null. Check the path of WeaponHolderSocket = {Constants.WeaponHolderSocket}");
        }
        _animationController = GetComponent<SamplePlayerAnimationController>();
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
        if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
        {
            int avaliableAmmunition = GetAvaliableAmmunitionForCurrentWeapon();
            if (avaliableAmmunition <= 0) return;

            rangedWeapon.StartReload();
        }
    }
    
    public void EquipItemInSlot(EquipmentSlots slot)
    {
        if (bIsEquipping) return;

        if (!weaponsArray.ContainsKey(slot) || weaponsArray[slot] == null)
        {
            Debug.LogWarning($"Slot {slot} is empty!");
            return;
        }

        Animator animator = GetComponent<SamplePlayerAnimationController>().GetAnimator();

        Weapon newWeapon = weaponsArray[slot];
        EquipmentSlots newSlot = slot;

        if (currentEquippedWeapon != null)
        {
            UnEquipCurrentItem();
        }
        else
        {
            EquipNewWeapon(newWeapon, newSlot);
        }
    }

    private void EquipNewWeapon(Weapon newWeapon, EquipmentSlots newSlot)
    {
        Animator animator = GetComponent<SamplePlayerAnimationController>().GetAnimator();

        currentEquippedWeapon = newWeapon;
        currentEquippedSlot = newSlot;

        if (currentEquippedWeapon != null)
        {
            bIsEquipping = true;
            animator.SetInteger(_animationController._weaponTypeHash, (int)currentEquippedWeapon.GetWeaponType());
            animator.ResetTrigger(_animationController._equipWeaponHash);
            animator.SetTrigger(_animationController._equipWeaponHash);
            
            StartCoroutine(WaitForEquipAnimation());
        }

        if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
        {
            rangedWeapon.OnAmmoChanged += OnCurrentWeaponAmmoChanged;
            rangedWeapon.OnReloadComplete += OnWeaponReloadComplete_Event;
            OnCurrentWeaponAmmoChanged(rangedWeapon.GetAmmo());
        }
    }

    public void OnEquipAnimationEnd()
    {
        bIsEquipping = false;
    }

    private IEnumerator WaitForEquipAnimation()
    {
        if (currentEquippedWeapon != null)
        {
            AnimationClip equipClip = currentEquippedWeapon.GetCharacterEquipClip();
            if (equipClip != null)
            {
                yield return new WaitForSeconds(equipClip.length);
            }
            else
            {
                Debug.LogWarning("equipClip is null");
                yield return new WaitForSeconds(1f);
            }
        }
        bIsEquipping = false;
    }

    public void ShowWeapon()
    {
        Debug.Log($"ShowWeapon {currentEquippedWeapon} called at: " + Time.time);
        if (currentEquippedWeapon != null)
        {
            AttachCurrentWeaponToEquippedSocket();
        }
    }

    public void AttachCurrentWeaponToEquippedSocket()
    {
        currentEquippedWeapon.gameObject.SetActive(true);
    }
    
    public void UnEquipCurrentItem()
    {
        Animator animator = GetComponent<SamplePlayerAnimationController>().GetAnimator();
        if (currentEquippedWeapon)
        {
            if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
            {
                rangedWeapon.StopFire();
                rangedWeapon.EndReload(false);
                rangedWeapon.OnAmmoChanged -= OnCurrentWeaponAmmoChanged;
                rangedWeapon.OnReloadComplete -= OnWeaponReloadComplete_Event;
            }
            animator.SetTrigger(_animationController._unequipWeaponHash);
            StartCoroutine(WaitForUnEquipAnimationAndEquip());
        }
    }

    private IEnumerator WaitForUnEquipAnimationAndEquip()
    {
        if (currentEquippedWeapon != null)
        {
            AnimationClip unEquipClip = currentEquippedWeapon.GetCharacterUnEquipClip();
            float waitTime = unEquipClip != null ? unEquipClip.length : 1f;
            yield return new WaitForSeconds(waitTime);
        }

        if (weaponsArray.ContainsKey(currentEquippedSlot) && weaponsArray[currentEquippedSlot] != null)
        {
            EquipNewWeapon(weaponsArray[currentEquippedSlot], currentEquippedSlot);
        }
    }

    public void HideWeapon()
    {
        Debug.Log($"HideWeapon {currentEquippedWeapon} called at: " + Time.time);
        if (currentEquippedWeapon != null)
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

        var validSlots = weaponsArray
            .Where(kvp => kvp.Value != null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (validSlots.Count == 0) return;

        int currentIndex = validSlots.IndexOf(currentEquippedSlot);
        if (currentIndex == -1) currentIndex = 0;

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
        if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
        {
            return ammunitionArray[rangedWeapon.GetAmmoType()];
        }
        return 0;
    }

    private void OnWeaponReloadComplete_Event()
    {
        if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
        {
            int avaliableAmmunition = GetAvaliableAmmunitionForCurrentWeapon();
            int currentAmmo = rangedWeapon.GetAmmo();
            int ammoToReload = rangedWeapon.GetMaxAmmo() - currentAmmo;
            int reloadedAmmo = Math.Min(avaliableAmmunition, ammoToReload);

            ammunitionArray[rangedWeapon.GetAmmoType()] -= reloadedAmmo;
            rangedWeapon.SetAmmo(reloadedAmmo + currentAmmo);
        }
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
        if (currentEquippedWeapon is IRangedWeapon rangedWeapon)
            OnCurrentWeaponAmmoChanged(rangedWeapon.GetAmmo());
    }

    public void UpgradeWeapon(WeaponType weaponType)
    {
        var existingSlots = weaponsArray
            .Where(kvp => kvp.Value != null && kvp.Value.GetWeaponType() == weaponType)
            .ToList();

        if (existingSlots.Count == 0)
        {
            CreateBaseWeaponIfNeeded(weaponType);
            existingSlots = weaponsArray
                .Where(kvp => kvp.Value != null && kvp.Value.GetWeaponType() == weaponType)
                .ToList();

            if (existingSlots.Count == 0)
            {
                Debug.LogError($"Failed to create base {weaponType}");
                return;
            }

            var newWeapon = existingSlots.First().Value;
            if (currentEquippedSlot == existingSlots.First().Key)
            {
                OnCurrentWeaponAmmoChanged((currentEquippedWeapon as IRangedWeapon)?.GetAmmo() ?? 0);
            }
            return;
        }

        var possibleUpgrades = upgradedWeapons
            .Where(u => u.weaponType == weaponType)
            .OrderBy(u => u.targetTier)
            .ToList();

        foreach (var slot in existingSlots)
        {
            EquipmentSlots eqSlot = slot.Key;
            Weapon oldWeapon = slot.Value;

            var nextTier = possibleUpgrades.FirstOrDefault(u =>
                (int)u.targetTier > (int)oldWeapon.weaponTier);

            if (nextTier == null) continue;

            Destroy(oldWeapon.gameObject);
            GameObject newGun = Instantiate(nextTier.upgradedPrefab, holderSocket, false);
            Weapon newWeapon = newGun.GetComponent<Weapon>();
            newWeapon.weaponTier = nextTier.targetTier;
            weaponsArray[eqSlot] = newWeapon;
            newGun.SetActive(false);

            if (currentEquippedSlot == eqSlot)
            {
                UnEquipCurrentItem();
                currentEquippedWeapon.gameObject.SetActive(true);
                currentEquippedWeapon = newWeapon;
                if (newWeapon is IRangedWeapon rangedNewWeapon)
                {
                    rangedNewWeapon.OnAmmoChanged += OnCurrentWeaponAmmoChanged;
                    rangedNewWeapon.OnReloadComplete += OnWeaponReloadComplete_Event;
                    OnCurrentWeaponAmmoChanged(rangedNewWeapon.GetAmmo());
                }
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
