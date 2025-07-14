using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;               // Referencia al objeto que la cámara debe seguir
    public float followSpeed = 5f;         // Velocidad de seguimiento

    // Opcional: límites de área
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Interpolamos suavemente hacia la posición del player
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Si se usan límites, se aplican
        if (useBounds)
        {
            float clampedX = Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
