using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private AnimatorController animatorController;

    private void Start()
    {
        animatorController = GetComponentInParent<AnimatorController>();
    }

    public void OnEndReload()
    {
        animatorController.ReturnRigWeight();
    }

    public void OnStartEquip()
    {
        animatorController.SetRigWeightToZero();
    }

    public void OnEndEquip()
    {
        animatorController.ReturnRigWeight();
    }
}
