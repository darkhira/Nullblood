using UnityEngine;

public class CombateCaC : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    [SerializeField]
    [Tooltip("Distancia desde el centro del jugador a la que se aplica el golpe.")]
    private float attackOffset = 1.0f;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Acepta un argumento Vector2 para saber la dirección del ataque.
    public void EjecutarGolpe(Vector2 attackDirection)
    {
        // 1. Mueve el "controladorGolpe" a la posición correcta ANTES de atacar.
        if (controladorGolpe != null)
        {
            controladorGolpe.localPosition = attackDirection.normalized * attackOffset;
        }

        // 2. Detecta colisionadores en la zona
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            // Verificamos si es un enemigo estándar
            if (colisionador.TryGetComponent<Enemigo>(out Enemigo enemigo))
            {
                enemigo.TomarDaño(playerStats.baseDamage);
            }
            // --- NUEVO: Verificamos si es el Jefe Kopp ---
            else if (colisionador.TryGetComponent<BossKopp>(out BossKopp jefe))
            {
                jefe.TomarDaño(playerStats.baseDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorGolpe != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
        }
    }
}