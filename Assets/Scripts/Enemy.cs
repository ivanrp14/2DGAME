using UnityEngine;
using Utils;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField]
    private GameObject dieEffectPrefab;

    // Protegidos para uso en subclases
    protected Animator animator;
    protected Health health;
    protected Transform player;
    protected float lastAttackTime;
    public GameObject expEffectPrefab;

    // Suscribir eventos al habilitar
    protected virtual void OnEnable()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("Health component not found on " + gameObject.name + ". Please add a Health component.");
        }
        if (health != null)
        {
            health.Hurt += OnHurt;
            health.OnZeroLifes += OnDie;
        }
    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Player not found in the scene. Make sure there's a GameObject with the 'Player' tag.");
        }


    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange && !health.isGettingDamage && health.GetCurrenthealth() > 0)
        {
            MoveTowardsPlayer();
        }
        else
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    // Movimiento hacia el jugador
    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        animator?.SetFloat("_speed", direction.magnitude);
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f); // Mirar a la izquierda
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Mirar a la derecha
        }
    }

    // Ataque al jugador
    protected virtual void Attack()
    {
        if (player == null) return;


    }

    // Evento cuando recibe daño
    protected virtual void OnHurt(float currentHealth)
    {

        if (animator != null)
        {
            animator.SetTrigger("_hit");
        }

        Debug.Log(gameObject.name + " recibió daño. Vida actual: " + currentHealth);
    }

    // Evento cuando muere
    protected virtual void OnDie()
    {
        if (dieEffectPrefab != null)
        {
            GameObject dieEffect = PoolManager.Instance.Get(dieEffectPrefab, transform.position + Vector3.up / 2f, Quaternion.identity);
        }
        if (animator != null)
        {
            animator.SetTrigger("_death");
        }
        if (expEffectPrefab != null)
            PoolManager.Instance.Get(expEffectPrefab, transform.position + Vector3.up / 2f, Quaternion.identity);
        Debug.Log(gameObject.name + " murió.");
        PoolManager.Instance.Return(gameObject, 1f);

    }

    // Desuscribir eventos al deshabilitar
    protected virtual void OnDisable()
    {
        if (health != null)
        {
            health.Hurt -= OnHurt;
            health.OnZeroLifes -= OnDie;
        }
    }

}
