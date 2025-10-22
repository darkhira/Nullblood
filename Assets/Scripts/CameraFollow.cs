using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target a seguir")]
    public Transform target;

    [Header("Ajustes de seguimiento")]
    public float smoothSpeed = 5f;   // Qué tan rápido sigue al jugador
    public Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}
