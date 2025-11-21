using UnityEngine;

public class AcidPool : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float duration = 5f; // Cuanto dura el charco antes de desaparecer

    private float damageTimer;

    void Start()
    {
        // El charco se destruye a sí mismo después de un tiempo
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Si el jugador pisa el charco
        if (other.CompareTag("Player"))
        {
            // Hacemos daño por "tick" (ej. cada 0.5 segundos) o por deltaTime
            damageTimer += Time.deltaTime;

            if (damageTimer >= 0.5f) // Daño cada medio segundo
            {
                // Buscamos el script PlayerStats (asegúrate de que el Player lo tenga)
                PlayerStats player = other.GetComponent<PlayerStats>();
                if (player != null)
                {
                    player.TakeDamage(damagePerSecond / 2, gameObject); // /2 porque es cada 0.5s
                }
                damageTimer = 0f;
            }
        }
    }
}