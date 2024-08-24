using TMPro;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weaponAmmoText;
    [SerializeField] private TextMeshProUGUI totalAmmoText;

    public void UpdateAmmoWidget(int weaponAmmo, int totalAmmo)
    {
        weaponAmmoText.SetText(weaponAmmo.ToString());
        totalAmmoText.SetText(totalAmmo.ToString());
    }
}
