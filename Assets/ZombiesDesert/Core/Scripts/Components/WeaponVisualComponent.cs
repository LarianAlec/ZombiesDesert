using Unity.VisualScripting;
using UnityEngine;

public class WeaponVisualComponent : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;

    private GameObject currentGun;
    private Animator animator;

    [Header("Left hand IK")]
    [SerializeField] private string IKTagKey = "leftHandIK";
    [SerializeField] private Transform leftHandTransform; 

    public void SwitchOffGuns()
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
        SwitchOn(0);
        SwitchAnimationLayer(2);
    }

    public void SwitchOnSecondSlot()
    {
        SwitchOn(1);
        SwitchAnimationLayer(3);
    }

    public void SwitchOnThirdSlot()
    {
        SwitchOn(2);
        SwitchAnimationLayer(4);
    }

    public void SwitchOnFourthSlot()
    {
        SwitchOn(3);
        SwitchAnimationLayer(5);
    }

    private void Start()
    {
        SwitchOffGuns();
        animator = GetComponentInChildren<Animator>();
    }

    private void AttachLeftHand()
    {
        Transform targetIKTransform = Helper.FindGameObjectInChildWithTag(currentGun, IKTagKey).transform;

        leftHandTransform.localPosition = targetIKTransform.localPosition;
        leftHandTransform.localRotation = targetIKTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        if (layerIndex > animator.layerCount)
        {
            return;
        }

        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }

        animator.SetLayerWeight(layerIndex, 1);
    }
}