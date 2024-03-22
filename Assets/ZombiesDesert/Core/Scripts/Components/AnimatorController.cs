using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimatorController : MonoBehaviour
{
    public Animator animator {  get; private set; } 
    private BaseCharacter cachedCharacter;
    private PlayerMovementComponent movementComponent;
    private int currentAnimationLayerIndex = 0;

    [Header("Inverse kinematic")]
    [SerializeField] private float rigWeightIncreaseRate = 3.0f;
    private Rig rig;
    private bool isRigWeightShoulBeIncreased = true;

    [Header("Character animations")]
    [SerializeField] private WeaponType currentEquippedWeaponType = WeaponType.None;
    [SerializeField] Transform socketLeftHandIK_Target;

    private void Start()
    {
        cachedCharacter = GetComponent<BaseCharacter>();
        animator = GetComponentInChildren<Animator>();
        movementComponent = cachedCharacter.GetMovementComponent();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        UpdateCurrentWeapon();
        UpdateRigWeigth();
        HandleAnimations();
    }

    private void UpdateRigWeigth()
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

    private void HandleAnimations()
    {
        float xVelocity = movementComponent.GetXVelocity();
        float zVelocity = movementComponent.GetZVelocity();

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool isRunning = movementComponent.GetMovementDirection().magnitude > 0.15f && movementComponent.IsRunningKeyPressed();
        animator.SetBool("isRunning", isRunning);
    }

#region Trigger animations
    public void RunShootAnimation()
    {
        animator.SetTrigger("MakeShoot");
    }

    public void RunReloadAnimation()
    {
        animator.SetTrigger("Reload");
        SetRigWeightToZero();
    }

    public void SetRigWeightToZero()
    {
        rig.weight = 0;
    }

    public void ReturnRigWeight()
    {
        isRigWeightShoulBeIncreased = true;
    }

    #endregion

    private void UpdateCurrentWeapon()
    {
        CharacterEquipmentComponent equipmentComponent = GetComponent<CharacterEquipmentComponent>();
        currentEquippedWeaponType = equipmentComponent.GetCurrentEquippedWeaponType();
        SwitchAnimationLayer(currentEquippedWeaponType);
        AttachLeftHand();
    }

    public int GetCurrentAnimationLayerIndex()
    {
        return currentAnimationLayerIndex;
    }

    private void SwitchAnimationLayer(WeaponType equippingWeapon)
    {
        switch (equippingWeapon)
        {
            case WeaponType.Pistol:
                SwitchOnPistolAnimationLayer();
                break;

            case WeaponType.Shotgun:
                SwitchOnShotgunAnimationLayer();
                break;

            case WeaponType.AutoRifle:
                SwitchOnAutoRifleAnimationLayer();
                break;

            case WeaponType.SniperRifle:
                SwitchOnSniperRifleAnimationLayer();
                break;
        }
    }

#region Switching animation layers
    private void SwitchOnPistolAnimationLayer()
    {
        SwitchOffAllAnimationLayers();
        animator.SetLayerWeight(Constants.PistolLayer, 1);
        currentAnimationLayerIndex = (int)Constants.PistolLayer;
    }

    private void SwitchOnShotgunAnimationLayer()
    {
        SwitchOffAllAnimationLayers();
        animator.SetLayerWeight(Constants.ShotgunLayer, 1);
        currentAnimationLayerIndex = (int)Constants.ShotgunLayer;
    }

    private void SwitchOnAutoRifleAnimationLayer()
    {
        SwitchOffAllAnimationLayers();
        animator.SetLayerWeight(Constants.AutoRifleLayer, 1);
        currentAnimationLayerIndex = (int)Constants.AutoRifleLayer;
    }

    private void SwitchOnSniperRifleAnimationLayer()
    {
        SwitchOffAllAnimationLayers();
        animator.SetLayerWeight(Constants.SniperLayer, 1);
        currentAnimationLayerIndex = (int)Constants.SniperLayer;
    }

    private void SwitchOffAllAnimationLayers()
    {
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
    }
#endregion

    private void AttachLeftHand()
    {
        GameObject currentGun = cachedCharacter.GetEquipmentComponent().GetCurrentEquippedWeapon().gameObject;
        Transform targetIKTransform = Helper.FindGameObjectInChildWithTag(currentGun, Constants.IKTagKey).transform;

        socketLeftHandIK_Target.localPosition = targetIKTransform.localPosition;
        socketLeftHandIK_Target.localRotation = targetIKTransform.localRotation;
    }
}
