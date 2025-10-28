using UnityEngine;

public class CurarAlTocar : MonoBehaviour
{
    [SerializeField] private int cantidadCuracion;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Buscamos el componente PlayerStats en lugar de VidaJugador
        if (col.TryGetComponent(out PlayerStats playerStats))
        {
            // Usamos el método Heal de PlayerStats
            playerStats.Heal(cantidadCuracion);
            Destroy(gameObject);
        }
    }
}