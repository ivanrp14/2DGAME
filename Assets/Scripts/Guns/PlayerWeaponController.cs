using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

public class PlayerWeaponController : MonoBehaviour
{
    public WeaponDataSO leftHandWeaponData;
    public WeaponDataSO rightHandWeaponData;

    public Transform aimCursor;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    public float cursorDistance = 2f;
    public float cursorSmoothSpeed = 20f;
    public float positionOffset = 0.1f;
    public float sideOffsetAmount = 0.15f;
    public float moveSpeed = 10f;

    public TextMeshProUGUI leftWeaponText;
    public TextMeshProUGUI rightWeaponText;

    private InputReader inputReader;
    private Vector2 lastAimDirection = Vector2.right;

    private Weapon leftHandWeaponInstance;
    private Weapon rightHandWeaponInstance;

    [Header("No weapon equipped")]
    [SerializeField] private float noWeaponDamage = 1f;
    [SerializeField] private float noWeaponFireRate = 0.5f;
    [SerializeField] private float noWeaponRange = 1f, noWeaponRadius = 0.3f;
    [SerializeField] private ParticleSystem noWeaponShootEffect;
    [SerializeField] private LayerMask enemyLayerMask;

    private Animator animator;
    private float lastNoWeaponAttackTime;

    void Awake()
    {
        inputReader = GetComponent<InputReader>();
        if (inputReader == null)
            Debug.LogError("No se encontró un InputReader en la escena.");
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        if (leftHandWeaponData != null)
            EquipLeftHandWeapon(leftHandWeaponData);
        if (rightHandWeaponData != null)
            EquipRightHandWeapon(rightHandWeaponData);
    }

    void Update()
    {
        UpdateAimCursor();
        AimWeapons();

        if (inputReader.IsShooting1 || inputReader.IsShooting2)
        {
            if (leftHandWeaponInstance != null && !leftHandWeaponInstance.isReloading && inputReader.IsShooting1)
            {
                leftHandWeaponInstance.TryShoot();
            }
            else if (rightHandWeaponInstance != null && !rightHandWeaponInstance.isReloading && inputReader.IsShooting2)
            {
                rightHandWeaponInstance.TryShoot();
            }

            else if (leftHandWeaponInstance == null && rightHandWeaponInstance == null)
            {
                AttackNoWeapon();
            }
        }

        if (inputReader.IsReloading)
        {
            if (leftHandWeaponInstance != null)
                StartCoroutine(leftHandWeaponInstance.Reload());
            if (rightHandWeaponInstance != null)
                StartCoroutine(rightHandWeaponInstance.Reload());
        }

        UpdateWeaponUI();
    }

    void AttackNoWeapon()
    {
        if (Time.time - lastNoWeaponAttackTime < noWeaponFireRate)
            return;

        lastNoWeaponAttackTime = Time.time;

        if (noWeaponShootEffect != null)
            noWeaponShootEffect.Play();

        animator.SetTrigger("_attack");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(noWeaponShootEffect.transform.position, noWeaponRadius, enemyLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider2D col in colliders)
            {
                Debug.Log("Golpeado: " + col.name);

                Health targetHealth = col.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(noWeaponDamage);
                    Debug.Log($"Daño infligido: {noWeaponDamage} a {col.name}");
                }
                else
                {
                    Debug.LogWarning("El objeto golpeado no tiene un componente Health.");
                }
            }
        }
        else
        {
            Debug.Log("No se golpeó a ningún enemigo.");
        }
    }

    void UpdateWeaponUI()
    {
        if (leftWeaponText != null)
            leftWeaponText.text = leftHandWeaponInstance != null
                ? (leftHandWeaponInstance.isReloading ? "Reloading..." : $"{leftHandWeaponInstance.currentAmmo}/{leftHandWeaponInstance.weaponData.maxAmmo}")
                : "No Weapon";

        if (rightWeaponText != null)
            rightWeaponText.text = rightHandWeaponInstance != null
                ? (rightHandWeaponInstance.isReloading ? "Reloading..." : $"{rightHandWeaponInstance.currentAmmo}/{rightHandWeaponInstance.weaponData.maxAmmo}")
                : "No Weapon";
    }

    void UpdateAimCursor()
    {
        Vector2 aimDir = inputReader.LastMoveDirection;
        if (aimDir.sqrMagnitude > 0.01f)
            lastAimDirection = aimDir.normalized;

        if (aimCursor != null)
        {
            Vector2 targetPos = (Vector2)transform.position + lastAimDirection * cursorDistance;
            aimCursor.position = Vector2.Lerp(aimCursor.position, targetPos, cursorSmoothSpeed * Time.deltaTime);
        }
    }

    void AimWeapons()
    {
        if (aimCursor == null) return;

        if (leftHandWeaponInstance != null && leftHandTransform != null)
        {
            Vector3 basePos = leftHandTransform.position;
            AddIdleOscillation(leftHandWeaponInstance, basePos);
            AimWeaponAtCursor(leftHandWeaponInstance);
        }

        if (rightHandWeaponInstance != null && rightHandTransform != null)
        {
            Vector3 basePos = rightHandTransform.position;
            AddIdleOscillation(rightHandWeaponInstance, basePos);
            AimWeaponAtCursor(rightHandWeaponInstance);
        }

        UpdateWeaponByLookDirection(leftHandWeaponInstance, rightHandWeaponInstance, sideOffsetAmount, moveSpeed);
    }

    void AimWeaponAtCursor(Weapon weapon)
    {
        Vector3 dir = (aimCursor.position - weapon.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0, 0, angle);

        var sr = weapon.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipY = (angle > 90f || angle < -90f);
    }

    void UpdateWeaponByLookDirection(Weapon weaponLeft, Weapon weaponRight, float sideOffsetAmount = 0.15f, float moveSpeed = 10f)
    {
        float dirX = Mathf.Sign(transform.localScale.x);

        if (weaponLeft != null)
        {
            weaponLeft.GetComponent<SpriteRenderer>().sortingOrder = (dirX < 0) ? -1 : 3;
            Vector3 targetPos = weaponLeft.transform.position;
            targetPos.x = leftHandTransform.position.x + (sideOffsetAmount * dirX * 0.7f);
            targetPos.y = weaponLeft.transform.position.y;
            weaponLeft.transform.position = Vector3.Lerp(weaponLeft.transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        if (weaponRight != null)
        {
            weaponRight.GetComponent<SpriteRenderer>().sortingOrder = (dirX < 0) ? 3 : -1;
            Vector3 targetPos = weaponRight.transform.position;
            targetPos.x = rightHandTransform.position.x + (sideOffsetAmount * dirX);
            targetPos.y = weaponRight.transform.position.y;
            weaponRight.transform.position = Vector3.Lerp(weaponRight.transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    void AddIdleOscillation(Weapon weapon, Vector3 basePos, float amplitude = 0.02f, float speed = 2f)
    {
        if (weapon == null) return;
        Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * speed + weapon.GetInstanceID()) * amplitude, 0);
        weapon.transform.position = basePos + offset;
    }

    public void EquipLeftHandWeapon(WeaponDataSO weaponData)
    {
        if (leftHandWeaponInstance != null)
            Destroy(leftHandWeaponInstance.gameObject);

        GameObject newWeaponGO = Instantiate(weaponData.weaponPrefab);
        leftHandWeaponInstance = newWeaponGO.GetComponent<Weapon>() ?? newWeaponGO.GetComponentInChildren<Weapon>();

        if (leftHandWeaponInstance == null)
        {
            Debug.LogError($"El prefab '{weaponData.weaponPrefab.name}' no tiene un componente Weapon.");
            Destroy(newWeaponGO);
            return;
        }

        leftHandWeaponInstance.weaponData = weaponData;
        leftHandWeaponData = weaponData;
    }

    public void EquipRightHandWeapon(WeaponDataSO weaponData)
    {
        if (rightHandWeaponInstance != null)
            Destroy(rightHandWeaponInstance.gameObject);

        GameObject newWeaponGO = Instantiate(weaponData.weaponPrefab);
        rightHandWeaponInstance = newWeaponGO.GetComponent<Weapon>() ?? newWeaponGO.GetComponentInChildren<Weapon>();

        if (rightHandWeaponInstance == null)
        {
            Debug.LogError($"El prefab '{weaponData.weaponPrefab.name}' no tiene un componente Weapon.");
            Destroy(newWeaponGO);
            return;
        }

        rightHandWeaponInstance.weaponData = weaponData;
        rightHandWeaponData = weaponData;
    }

    public void DropLeftHandWeapon()
    {
        if (leftHandWeaponInstance != null)
        {
            leftHandWeaponInstance.Drop();
            leftHandWeaponInstance = null;
        }
        leftHandWeaponData = null;
    }

    public void DropRightHandWeapon()
    {
        if (rightHandWeaponInstance != null)
        {
            rightHandWeaponInstance.Drop();
            rightHandWeaponInstance = null;
        }
        rightHandWeaponData = null;
    }

    public void DropAllWeapons()
    {
        DropLeftHandWeapon();
        DropRightHandWeapon();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(noWeaponShootEffect.transform.position, noWeaponRadius);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            WeaponDataSO weaponData = collision.GetComponent<Weapon>()?.weaponData;
            if (weaponData != null)
            {
                if (leftHandWeaponInstance == null)
                {
                    EquipLeftHandWeapon(weaponData);
                }
                else if (rightHandWeaponInstance == null)
                {
                    EquipRightHandWeapon(weaponData);
                }
                else
                {
                    Debug.LogWarning("Ya tienes dos armas equipadas. No se puede recoger más.");
                }
            }
            Destroy(collision.gameObject);
        }
    }
}
