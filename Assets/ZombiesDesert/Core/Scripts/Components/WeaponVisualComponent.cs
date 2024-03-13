using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualComponent : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;

    private GameObject currentGun;
    private Animator animator;
    private Rig rig;

    [Header("Rig")]
    [SerializeField] private float rigIncreaseStep;
    private bool isRigWeightShoulBeIncreased;

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

    public void PlayReloadAnimation()
    {
        animator.SetTrigger("Reload");
        rig.weight = 0.0f;
    }

    public void SetRigWeightToOne() => isRigWeightShoulBeIncreased = true;

    private void Start()
    {
        SwitchOffGuns();
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        if (isRigWeightShoulBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;

            if (rig.weight >= 1.0f)
            {
                isRigWeightShoulBeIncreased = false;
            }
        }
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