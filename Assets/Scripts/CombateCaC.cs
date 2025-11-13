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

    // --- �M�TODO MODIFICADO! ---
    // Acepta un argumento Vector2 para saber la direcci�n del ataque.
    public void EjecutarGolpe(Vector2 attackDirection)
    {
        // 1. Mueve el "controladorGolpe" a la posici�n correcta ANTES de atacar.
        if (controladorGolpe != null)
        {
            controladorGolpe.localPosition = attackDirection.normalized * attackOffset;
        }

        // 2. Detecta enemigos en la NUEVA posici�n del controlador
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                if (colisionador.TryGetComponent<Enemigo>(out Enemigo enemigo))
                {
                    enemigo.TomarDaño(playerStats.baseDamage);
                }
            }
        }
    }

    // Dibuja el gizmo en la posici�n del controlador para depuraci�n
    private void OnDrawGizmos()
    {
        if (controladorGolpe == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}