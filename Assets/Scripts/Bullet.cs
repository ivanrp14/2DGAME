using UnityEngine;
using Utils;
using System.Collections;

public class Bullet : MonoBehaviour, IPoolable
{
    public float damage = 10f;
    public GameObject hitEffect, bloodEffect;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!IsOnScreen())
            ReturnToPool();
    }

    private bool IsOnScreen()
    {
        if (mainCamera == null) return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;
    }

    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Evita ejecuciones múltiples
        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            Debug.Log("Bullet hit an enemy: " + collision.gameObject.name);
            if (collision.gameObject.TryGetComponent(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Enemy does not have a Health component.");
            }

            if (hitEffect != null)
            {
                GameObject effect = PoolManager.Instance.Get(hitEffect, transform.position, Quaternion.identity);
            }

            if (bloodEffect != null)
            {
                GameObject effect = PoolManager.Instance.Get(bloodEffect, transform.position + Vector3.up / 2f, Quaternion.identity);
            }
            else
            {
                Debug.Log("error blood");
            }

            ReturnToPool();
        }
    }


    private IEnumerator ReturnEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.Return(effect);
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.Return(gameObject);
    }

    public void OnReturnToPool()
    {
        hasHit = false; // Resetea el estado para que pueda ser reutilizado4
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
