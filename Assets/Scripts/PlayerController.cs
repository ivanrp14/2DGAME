using UnityEngine;
using Utils;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    public float maxMoveSpeed = 10f;
    public float minMoveSpeed = 1f;
    public float speedSmoothTime = 0.1f;

    private Rigidbody2D rb;
    private InputReader inputReader;
    private Animator animator;

    private float currentSpeed = 0f;
    private Vector3 originalScale;
    public Health health;
    public ExperienceSystem experienceSystem;
    void Awake()
    {
        experienceSystem = GetComponent<ExperienceSystem>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        inputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        if (inputReader == null)
        {
            Debug.LogError("No se encontró un InputReader en la escena.");
        }
    }
    void OnEnable()
    {
        if (health != null)
        {
            health.Hurt += OnHurt;
            health.OnZeroLifes += OnDie;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        if (inputReader == null) return;

        Vector2 moveInput = inputReader.MovementValue;

        // Movimiento proporcional a la intensidad del joystick o input
        Vector2 movement = moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Suavizar el valor de velocidad para la animación
        float targetSpeed = moveInput.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedSmoothTime);
        animator.SetFloat("_speed", currentSpeed);

        // Voltear el objeto si se mueve en X
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput.x) * Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }
    void OnHurt(float damage)
    {
        animator.SetTrigger("_hit");
    }
    private void OnDie()
    {

        animator.SetTrigger("_death");
        // Aquí puedes agregar lógica para eliminar al enemigo del juego, como desactivarlo o destruirlo
        Destroy(gameObject, 1f); // Destruye el enemigo después de 2 segundos
    }
    private void OnDisable()
    {
        if (health != null)
        {
            health.Hurt -= OnHurt;
            health.OnZeroLifes -= OnDie;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exp"))
        {
            experienceSystem.AddExperience(10);
            PoolManager.Instance.Return(collision.gameObject);
        }

    }
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Clamp(newSpeed, minMoveSpeed, maxMoveSpeed);
        animator.SetFloat("_moveSpeed", moveSpeed);
    }
    public float GetSpeed()
    {
        return moveSpeed;
    }

}
