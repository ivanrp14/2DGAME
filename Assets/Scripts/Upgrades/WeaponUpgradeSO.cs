using UnityEngine;

public enum WeaponUpgradeType
{
    IncreaseDamage,
    IncreaseFireRate,
    IncreaseMagazineSize,
    ReduceReloadTime,
    UnlockWeapon,
}

[CreateAssetMenu(fileName = "NewWeaponUpgrade", menuName = "Upgrades/Weapon Upgrade")]
public class WeaponUpgradeSO : UpgradeSO
{
    public WeaponDataSO targetWeapon;
    public WeaponUpgradeType upgradeType;
}
