using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null)
        {
            // Sigue la posición X y Y del jugador, pero mantiene su propia altura Z (o Y en 3D)
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; // Mantener la altura de la cámara (Z en 2D suele ser -10)
            transform.position = newPosition;
        }
    }
}