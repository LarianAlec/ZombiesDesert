using System;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    public WeaponTier weaponTier;
    public Transform leftHandIKSoket;
    public BaseCharacter characterOwner;
    [SerializeField] private AnimationClip equipClip;
    [SerializeField] private AnimationClip unEquipClip;

    private void InitializeCharacterOwner()
    {
        if (characterOwner == null)
        {
            characterOwner = Helper.TryGetCharacterOwner(this.gameObject);
        }
    }

    public AnimationClip GetCharacterEquipClip() => equipClip;
    public AnimationClip GetCharacterUnEquipClip() => unEquipClip;
    public BaseCharacter GetCharacterOwner() => characterOwner;
    public void SetOwner(BaseCharacter newOwner) => characterOwner = newOwner;
    public abstract WeaponType GetWeaponType();
    public abstract void StartAction();
    public abstract void StopAction();
}
