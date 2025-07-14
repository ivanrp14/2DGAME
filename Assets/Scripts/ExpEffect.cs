using UnityEngine;

public class ExpEffect : MonoBehaviour, IPoolable
{
    public float detectDistance = 5f;      // Distancia a la que detecta al jugador
    public float moveSpeed = 3f;           // Velocidad a la que se mueve hacia el jugador
    public float minScale = 0.2f;          // Escala mínima al acercarse
    public float scaleSpeed = 5f;          // Qué tan rápido se ajusta la escala

    private Transform player;
    private float originalScale;

    public void OnReturnToPool()
    {
        // Resetear escala al devolver al pool
        transform.localScale = Vector3.one * originalScale;
    }

    void Start()
    {
        originalScale = transform.localScale.x; // Guardar escala original
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectDistance)
        {
            // Mover hacia el jugador
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // Reducir escala en proporción a la distancia
            float scaleFactor = Mathf.Clamp01(distance / detectDistance); // De 1 a 0
            float newScale = Mathf.Lerp(minScale, 1f, scaleFactor);       // De minScale a 1
            transform.localScale = Vector3.one * newScale;
        }
    }
}
