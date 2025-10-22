using UnityEngine;

public class DañoAlTocar : MonoBehaviour
{
    [SerializeField] private int dañoPorToque = 10;
    [SerializeField] private float tiempoEntreDaño = 1f;

    private float tiempoSiguienteDaño;

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerStats playerStats))
        {
            if (Time.time >= tiempoSiguienteDaño)
            {
                playerStats.TakeDamage(dañoPorToque, gameObject);

                // Reiniciamos el temporizador para el siguiente golpe.
                tiempoSiguienteDaño = Time.time + tiempoEntreDaño;
            }
        }
    }
}