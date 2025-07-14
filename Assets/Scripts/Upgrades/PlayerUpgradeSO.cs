using UnityEngine;

public enum PlayerUpgradeType
{
    MaxHealth,
    MoveSpeed,
}

[CreateAssetMenu(fileName = "NewStatUpgrade", menuName = "Upgrades/Stat Upgrade")]
public class PlayerUpgradeSO : UpgradeSO
{
    public PlayerUpgradeType upgradeType;
}
