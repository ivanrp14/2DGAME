using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponDataSO weaponData;
    public Transform firePoint;

    private float lastShootTime;
    public bool isReloading = false;
    public int currentAmmo;
    public bool isPickable = true;

    public float aimFollowSpeed = 15f;
    private Vector3 targetDirection = Vector3.right;

    private void Start()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    private void Update()
    {
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, targetDirection));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, aimFollowSpeed * Time.deltaTime);
        }
    }

    public void SetAimDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
            targetDirection = direction.normalized;
    }

    public void TryShoot()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        float fireCooldown = 1f / weaponData.fireRate;
        if (Time.time - lastShootTime >= fireCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
            currentAmmo--;
        }
    }

    private void Shoot()
    {
        switch (weaponData.weaponType)
        {
            case WeaponType.Pistol:
                FireBullet();
                break;
            case WeaponType.Shotgun:
                FireShotgun();
                break;
            case WeaponType.Burst:
                FireBullet();
                break;
        }
    }

    private void FireBullet()
    {
        GameObject bullet = PoolManager.Instance.Get(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet == null) return;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        if (bulletComponent != null)
            bulletComponent.damage = weaponData.damage;

        if (rb != null)
            rb.linearVelocity = firePoint.right * weaponData.bulletSpeed;
    }

    private void FireShotgun()
    {
        float baseAngle = firePoint.eulerAngles.z;
        float halfSpread = weaponData.spreadAngle / 2f;

        for (int i = 0; i < weaponData.pellets; i++)
        {
            float randomOffset = Random.Range(-halfSpread, halfSpread);
            float finalAngle = baseAngle + randomOffset;
            Vector2 finalDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            GameObject bullet = PoolManager.Instance.Get(weaponData.bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet == null) continue;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            if (bulletComponent != null)
                bulletComponent.damage = weaponData.damage;

            if (rb != null)
                rb.linearVelocity = finalDir.normalized * weaponData.bulletSpeed;
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log($"Recargando {weaponData.weaponName}...");

        yield return new WaitForSeconds(weaponData.reloadTime);

        currentAmmo = weaponData.maxAmmo;
        Debug.Log($"{weaponData.weaponName} recargada.");

        isReloading = false;
    }
    public void Drop()
    {
        Debug.Log($"Dropping weapon: {weaponData.weaponName}");
        Destroy(gameObject);
    }
}
