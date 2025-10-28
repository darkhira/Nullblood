using UnityEngine;

public class CombateCaC : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;

    // --- CAMBIO 1: Eliminamos las variables que ahora estï¿½n en PlayerStats ---
    // [SerializeField] private float daï¿½oGolpe;
    // [SerializeField] private float tiempoEntreAtaques;

    private float tiempoSiguienteAtaque;
    private Animator anim;

    // --- CAMBIO 2: Aï¿½adimos una referencia a PlayerStats ---
    private PlayerStats playerStats;

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Asumimos que este script y PlayerStats estï¿½n en el mismo objeto (el Jugador)
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (tiempoSiguienteAtaque > 0)
        {
            tiempoSiguienteAtaque -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 0)
        {
            Golpe();
            // --- CAMBIO 3: Calculamos el cooldown basado en el attackSpeed de PlayerStats ---
            // attackSpeed es "ataques por segundo", asï¿½ que el tiempo entre ataques es 1 / attackSpeed
            tiempoSiguienteAtaque = 1f / playerStats.attackSpeed;
        }
    }

    public void Golpe()
    {
        // Detectamos a los enemigos en el área de golpe
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                // --- CAMBIO 4: Usamos el baseDamage de PlayerStats para el daï¿½o ---
                colisionador.transform.GetComponent<Enemigo>().TomarDaño(playerStats.baseDamage);
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