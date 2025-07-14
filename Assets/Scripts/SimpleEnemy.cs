using UnityEngine;
using Utils;

public class SimpleEnemy : Enemy
{
    [SerializeField]
    private ParticleSystem attackPS;
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private LayerMask playerLayerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Attack()
    {
        base.Attack();
        Debug.Log("No weapons equipped, using no-weapon area attack.");

        if (attackPS != null)
            attackPS.Play();

        // Detectar todos los enemigos en un área circular alrededor del jugador
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius, playerLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider2D col in colliders)
            {
                Debug.Log("Golpeado: " + col.name);

                Health targetHealth = col.GetComponent<Health>();


                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                    Debug.Log($"Daño infligido: {damage} a {col.name}");
                }
                else
                {
                    Debug.LogWarning("El objeto golpeado no tiene un componente Health.");
                }
            }
        }


        return;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPS.transform.position, attackRadius);
    }
}

