using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualComponent : MonoBehaviour
{

    private Animator animator;
    private Rig rig;
    private bool isEquippingWeapon = false;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate = 3.0f;
    private bool isRigWeightShoulBeIncreased;

    
    [SerializeField] private Transform leftHandTransform;

    private void Start()
    {
        //SwitchOffGuns();
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        UpdateRigWeight();
    }

    public void SetRigWeightToZero() => rig.weight = 0;

    public void SetRigWeightToOne() => isRigWeightShoulBeIncreased = true;

    /*public void SwitchOffGuns()
    {
        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }
    }

    public void SwitchOn(int slotIndex)
    {
        if (guns.Length < slotIndex)
        {
            return;
        }

        SwitchOffGuns();
        currentGun = guns[slotIndex];
        currentGun.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOnFirstSlot()
    {
        RunEquipAnimation();
        SwitchOn(0);
        SwitchAnimationLayer(2);
    }
*/



    public void OnEquipAnimationEnd_Impl()
    {
        isEquippingWeapon = false;
        animator.SetBool("IsEquippingWeapon", isEquippingWeapon);
    }

    

    private void UpdateRigWeight()
    {
        if (isRigWeightShoulBeIncreased)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1.0f)
            {
                isRigWeightShoulBeIncreased = false;
            }
        }
    }

    

   

    private void RunEquipAnimation()
    {
        SetRigWeightToZero();
        animator.SetTrigger("EquipWeapon");

        isEquippingWeapon = true;

        animator.SetBool("IsEquippingWeapon", isEquippingWeapon);
    }
}