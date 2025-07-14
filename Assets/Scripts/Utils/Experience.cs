using UnityEngine;
using System;
using Microlight.MicroBar;
using TMPro;

public class ExperienceSystem : MonoBehaviour
{
    // Evento para notificar cambio de nivel
    public event Action<int> OnLevelUp;

    [Header("XP Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;

    // XP necesaria para el siguiente nivel
    public int xpToNextLevel = 100;

    // Factor de crecimiento del XP requerido (ej: 1.2 = 20% más cada nivel)
    public float xpGrowthFactor = 1.2f;
    public MicroBar expBar;
    public TextMeshProUGUI levelText;
    public float multiplier = 1f; // Multiplicador de XP (puede ser usado para eventos especiales)
    public int comboMultiplier = 1; // Multiplicador de combo (puede ser usado para eventos especiales)

    private void Start()
    {
        // Inicializar la barra de experiencia
        if (expBar != null)
        {
            expBar.Initialize(xpToNextLevel);
            expBar.UpdateBar(currentXP);
        }
    }


    public void AddExperience(int amount)
    {
        currentXP += amount * comboMultiplier * Mathf.RoundToInt(multiplier);
        if (expBar != null)
        {
            expBar.UpdateBar(currentXP);
        }
        Debug.Log($"Experiencia añadida: {amount}. XP actual: {currentXP}");
        // Repetir subida de nivel si se pasa de XP más de un nivel
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpGrowthFactor);
        expBar.SetNewMaxHP(xpToNextLevel);
        expBar.UpdateBar(0);
        Debug.Log($"¡Subiste al nivel {currentLevel}!");
        UpdateLevelText();
        OnLevelUp?.Invoke(currentLevel);
    }

    // Para obtener progreso entre niveles (0 a 1)
    public float GetXPProgressNormalized()
    {
        return (float)currentXP / xpToNextLevel;
    }
    private void UpdateLevelText()
    {
        levelText.text = "Level " + currentLevel;
    }

}
