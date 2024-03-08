using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("MakeShoot");
    }
}
