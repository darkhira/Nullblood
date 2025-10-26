using UnityEngine;

public class CombateCaC : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;

    private PlayerStats playerStats;

    private void Start()
    {
        // Obtenemos la referencia a las estad�sticas del jugador
        playerStats = GetComponent<PlayerStats>();
    }

    // --- M�TODO P�BLICO ---
    // Este m�todo ser� llamado por PlayerMovement para ejecutar el ataque.
    public void EjecutarGolpe()
    {
        // Detectamos a los enemigos en el �rea de golpe
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                colisionador.transform.GetComponent<Enemigo>().TomarDa�o(playerStats.baseDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorGolpe == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}