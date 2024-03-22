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
    }

    private bool bIsEquipping = false;

    [Header("Weapons")]
    [SerializeField] private Transform weaponSocket;
    [SerializeField] protected AmmunitionAmount[] startAmmunition;
    [SerializeField] protected WeaponsArray[] startWeapons;
    [SerializeField] protected EquipmentSlots autoEquipSlot;

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
        if (bIsEquipping)
        {
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
        int currentSlotIndex = (int)currentEquippedSlot;
        int nextSlotIndex = NextWeaponArraySlotIndex(currentSlotIndex);

        while (currentSlotIndex != nextSlotIndex && !weaponsArray[(EquipmentSlots)nextSlotIndex])
        {
            nextSlotIndex = NextWeaponArraySlotIndex(nextSlotIndex);
        }

        if (currentSlotIndex != nextSlotIndex)
        {
            EquipItemInSlot((EquipmentSlots)nextSlotIndex);
        }
    }

    public void EquipPreviousItem()
    {
        int currentSlotIndex = (int)currentEquippedSlot;
        int previousSlotIndex = PreviousWeaponArraySlotIndex(currentSlotIndex);

        while (currentSlotIndex != previousSlotIndex && weaponsArray[(EquipmentSlots)previousSlotIndex] != null)
        {
            previousSlotIndex = PreviousWeaponArraySlotIndex(previousSlotIndex);
        }

        if (currentSlotIndex != previousSlotIndex)
        {
            EquipItemInSlot((EquipmentSlots)previousSlotIndex);
        }
    }

    private int NextWeaponArraySlotIndex(int currentSlotIndex)
    {
        if (currentSlotIndex == weaponsArray.Count - 1)
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
        if (currentSlotIndex == 0)
        {
            return weaponsArray.Count - 1;
        }
        else
        {
            return currentSlotIndex - 1;
        }
    }

    private void AutoEquip()
    {
        if (autoEquipSlot != EquipmentSlots.None)
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
            GameObject gunObj = Instantiate(weaponPair.gunPrefab, holderSocket, false);
            Weapon gunComponent = gunObj.GetComponent<Weapon>();
            weaponsArray.Add(weaponPair.equipmentSlot, gunComponent);
            gunObj.SetActive(false);
        }

        Debug.Log("WeaponsArray : " +string.Join(',', weaponsArray));
        Debug.Log("AmmoLoadout : "+ string.Join(',', ammunitionArray));
    }

    private int GetAvaliableAmmunitionForCurrentWeapon()
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


    IEnumerator SetEquippingBoolToFalse(float time)
    {
        bIsEquipping = false;
        yield return new WaitForSeconds(time);
    }
}
