using UnityEngine;

public enum WeaponType
{
    Pistol,
    Shotgun,
    Burst,
    Missile,
    Laser
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Datos Generales")]
    [Header("Estado de Desbloqueo")]
    public bool isUnlocked = false;

    public string weaponName;
    public Sprite weaponIcon;               // Para UI
    public GameObject weaponPrefab, weaponPickupPrefab;  // Prefab para pickups en el mundo
    public WeaponType weaponType;         // Tipo de arma
    [Header("Ammo")]
    public int maxAmmo = 30;
    [Header("Reload Time")]           // Munición máxima
    public float reloadTime = 2f, minReloadTime = 0.5f, maxReloadTime = 5f;
    public bool isAutomatic = false;
    public float damage = 10f, minDamage = 1f, maxDamage = 1000f;              // Daño por disparo

    // Nombre del pool para el sistema de pooling
    [Header("Datos de Proyectil")]
    public string poolName;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;         // Velocidad del proyectil
    public float fireRate = 1f, minFireRate = 0.1f, maxFireRate = 1f;             // Disparos por segundo

    [Header("Configuración para Ráfaga (Burst)")]
    [Tooltip("Solo válido para armas tipo Burst")]
    public int burstCount = 3, maxBurstCount = 10, minBurstCount = 1;               // Número de disparos por ráfaga
    [Tooltip("Tiempo entre disparos en ráfaga")]
    public float burstDelay = 0.1f, minBurstDelay = 0.05f, maxBurstDelay = 0.5f; // Tiempo entre disparos en ráfaga

    [Header("Configuración para Escopeta (Shotgun)")]
    [Tooltip("Número de balas por disparo")]
    public int pellets = 8, minPellets = 1, maxPellets = 10;                  // Solo para escopetas
    [Tooltip("Ángulo total de dispersión en grados")]
    public float spreadAngle = 45f;          // Solo para escopetas

    public void SetReloadTime(float newReloadTime)
    {
        reloadTime = Mathf.Clamp(newReloadTime, minReloadTime, maxReloadTime);
    }
    public void SetDamage(float newDamage)
    {
        damage = Mathf.Clamp(newDamage, minDamage, maxDamage);
    }
    public void SetFireRate(float newFireRate)
    {
        fireRate = Mathf.Clamp(newFireRate, minFireRate, maxFireRate);
    }
    public void SetBurstCount(int newBurstCount)
    {
        burstCount = Mathf.Clamp(newBurstCount, 1, maxBurstCount);
    }
    public void SetBurstDelay(float newBurstDelay)
    {
        burstDelay = Mathf.Clamp(newBurstDelay, minBurstDelay, maxBurstDelay);
    }
    public void SetPellets(int newPellets)
    {
        pellets = Mathf.Clamp(newPellets, minPellets, maxPellets);
    }
    public void SetSpreadAngle(float newSpreadAngle)
    {
        spreadAngle = Mathf.Clamp(newSpreadAngle, 0f, 180f); // Asegurarse de que el ángulo esté entre 0 y 180 grados
    }
    // Puedes agregar métodos de validación para evitar que se usen mal las variables, por ejemplo:
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Resetear valores que no corresponden según el tipo
        if (weaponType != WeaponType.Burst)
        {
            burstCount = 0;
            burstDelay = 0f;
        }

        if (weaponType != WeaponType.Shotgun)
        {
            pellets = 0;
            spreadAngle = 0f;
        }
    }
#endif
}
