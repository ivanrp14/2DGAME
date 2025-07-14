using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    public List<WeaponDataSO> allWeapons;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockWeapon(WeaponDataSO weapon)
    {
        if (weapon != null)
        {
            weapon.isUnlocked = true;
            Debug.Log($"Arma {weapon.weaponName} desbloqueada.");
        }
    }

    public bool IsWeaponUnlocked(WeaponDataSO weaponData)
    {
        return weaponData != null && weaponData.isUnlocked;
    }

    public void ApplyWeaponUpgrade(WeaponUpgradeSO upgrade)
    {
        var weapon = upgrade.targetWeapon;
        if (weapon == null)
        {
            Debug.LogWarning("Upgrade sin arma objetivo.");
            return;
        }

        if (!weapon.isUnlocked && upgrade.upgradeType != WeaponUpgradeType.UnlockWeapon)
        {
            Debug.LogWarning($"El arma {weapon.weaponName} no está desbloqueada.");
            return;
        }

        switch (upgrade.upgradeType)
        {
            case WeaponUpgradeType.IncreaseDamage:
                weapon.SetDamage(weapon.damage + upgrade.value);
                break;
            case WeaponUpgradeType.IncreaseFireRate:
                weapon.SetFireRate(weapon.fireRate + upgrade.value);
                break;
            case WeaponUpgradeType.IncreaseMagazineSize:
                weapon.maxAmmo += Mathf.RoundToInt(upgrade.value);
                break;
            case WeaponUpgradeType.ReduceReloadTime:
                weapon.SetReloadTime(weapon.reloadTime - upgrade.value);
                break;
            case WeaponUpgradeType.UnlockWeapon:
                UnlockWeapon(weapon);
                break;
        }

        Debug.Log($"Upgrade {upgrade.upgradeName} aplicado a {weapon.weaponName}");
    }
}
