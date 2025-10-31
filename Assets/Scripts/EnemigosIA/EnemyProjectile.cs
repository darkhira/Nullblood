using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f; // Tiempo antes de autodestruirse si no golpea nada

    void Start()
    {
        // Se autodestruye después de 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprueba si ha chocado con el jugador
        if (other.CompareTag("Player"))
        {
            // Busca el script PlayerStats y le aplica daño
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage, gameObject);
            }
            Destroy(gameObject); // Destruye el proyectil al impactar
        }
        // Comprueba si ha chocado con una pared
        else if (other.CompareTag("Paredes"))
        {
            Destroy(gameObject); // Destruye el proyectil al impactar
        }
    }
}