using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UIUpgrades : MonoBehaviour
{
    public TextMeshProUGUI[] upgradeText;
    public Button[] upgradeButtons;
    public GameObject upgradesPanel;

    void Start()
    {
        UpgradeManager.Instance.OnUpgradesOffered += SetUpgrades;
        upgradesPanel.SetActive(false); // Inicialmente ocultamos el panel de mejoras
    }
    private void SetUpgrades(List<UpgradeSO> upgrades)
    {
        upgradesPanel.SetActive(true);

        for (int i = 0; i < upgradeText.Length; i++)
        {
            Debug.Log("Setting upgrade text for index: " + i);
            if (i < upgrades.Count)
            {
                var upgrade = upgrades[i];  // <-- Capture by value here
                upgradeText[i].text = upgrade.upgradeName + "\n" + upgrade.description;
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgrade));
            }
            else
            {
                upgradeText[i].text = "";
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        Time.timeScale = 0f;
    }



    private void ApplyUpgrade(UpgradeSO upgradeSO)
    {
        UpgradeManager.Instance.ApplyUpgrade(upgradeSO);
        Debug.Log("Applied Upgrade: " + upgradeSO.upgradeName);
        Time.timeScale = 1f; // Reset time scale after applying upgrade
        upgradesPanel.SetActive(false); // Hide the upgrade panel
    }

}
