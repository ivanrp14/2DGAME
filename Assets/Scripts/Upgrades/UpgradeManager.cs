using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Upgrades Disponibles")]
    public List<UpgradeSO> availableUpgrades;

    public WeaponManager weaponManager;
    private PlayerController playerController;

    public Action<List<UpgradeSO>> OnUpgradesOffered;
    public Action OnUpgradesShow;

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

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("No se encontró un PlayerController en la escena.");
        }
        else
        {
            playerController.experienceSystem.OnLevelUp += OfferUpgrades;
        }
    }

    public void OfferUpgrades(int level)
    {
        List<UpgradeSO> possibleUpgrades = new List<UpgradeSO>();

        foreach (var upgrade in availableUpgrades)
        {
            if (upgrade is WeaponUpgradeSO weaponUpgrade)
            {
                if (weaponUpgrade.upgradeType == WeaponUpgradeType.UnlockWeapon ||
                    weaponManager.IsWeaponUnlocked(weaponUpgrade.targetWeapon))
                {
                    possibleUpgrades.Add(upgrade);
                }
            }
            else
            {
                possibleUpgrades.Add(upgrade);
            }
        }

        var selectedUpgrades = RandomUtils.ChooseRandomElements(possibleUpgrades, 4);

        OnUpgradesShow?.Invoke();
        OnUpgradesOffered?.Invoke(selectedUpgrades);

        Debug.Log("Ofreciendo upgrades:");
        foreach (var upgrade in selectedUpgrades)
            Debug.Log($"- {upgrade.upgradeName}");
    }

    public void ApplyUpgrade(UpgradeSO upgrade)
    {
        if (upgrade is PlayerUpgradeSO playerUpgrade)
        {
            ApplyPlayerUpgrade(playerUpgrade);
        }
        else if (upgrade is WeaponUpgradeSO weaponUpgrade)
        {
            weaponManager.ApplyWeaponUpgrade(weaponUpgrade);
        }
        else
        {
            Debug.LogWarning($"Tipo de upgrade no manejado: {upgrade.GetType()}");
        }
    }

    void ApplyPlayerUpgrade(PlayerUpgradeSO playerUpgrade)
    {
        switch (playerUpgrade.upgradeType)
        {
            case PlayerUpgradeType.MaxHealth:
                playerController.health.SetMaxHealth(playerController.health.GetMaxHealth() + playerUpgrade.value);
                playerController.health.Heal(playerUpgrade.value);
                break;
            case PlayerUpgradeType.MoveSpeed:
                playerController.SetSpeed(playerController.GetSpeed() + playerUpgrade.value);
                break;
            default:
                Debug.LogWarning("Tipo de upgrade de jugador desconocido: " + playerUpgrade.upgradeType);
                break;
        }

        Debug.Log($"Aplicado upgrade de jugador: {playerUpgrade.upgradeName}");
    }
}
