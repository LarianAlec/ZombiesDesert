using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualComponent : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform firstWeaponTransform;
    [SerializeField] private Transform secondWeaponTransform;
    [SerializeField] private Transform thirdWeaponTransform;

    public void SwitchOffGuns()
    {
        foreach (Transform gun in gunTransforms)
        {
            gun.gameObject.SetActive(false);
        }
    }

    public void SwitchOnFirstSlot()
    {
        if (gunTransforms.Length < 0)
        {
            return;
        }
        SwitchOffGuns();
        gunTransforms[0].gameObject.SetActive(true);
    }

    public void SwitchOnSecondSlot()
    {
        if (gunTransforms.Length < 1)
        {
            return;
        }
        SwitchOffGuns();
        gunTransforms[1].gameObject.SetActive(true);
    }

    public void SwitchOnThirdSlot()
    {
        if (gunTransforms.Length < 2)
        {
            return;
        }
        SwitchOffGuns();
        gunTransforms[2].gameObject.SetActive(true);
    }

    private void Start()
    {
        SwitchOffGuns();
    }
}
