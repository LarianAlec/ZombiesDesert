using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private WeaponVisualComponent visualComponent;

    private void Start()
    {
        visualComponent = GetComponentInParent<WeaponVisualComponent>();
    }

    public void OnReloadOver()
    {
        visualComponent.SetRigWeightToOne();
    }
}
