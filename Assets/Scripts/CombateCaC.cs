using UnityEngine;

public class CombateCaC : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;

    // --- CAMBIO 1: Eliminamos las variables que ahora est�n en PlayerStats ---
    // [SerializeField] private float da�oGolpe;
    // [SerializeField] private float tiempoEntreAtaques;

    private float tiempoSiguienteAtaque;
    private Animator anim;

    // --- CAMBIO 2: A�adimos una referencia a PlayerStats ---
    private PlayerStats playerStats;

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Asumimos que este script y PlayerStats est�n en el mismo objeto (el Jugador)
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
            // attackSpeed es "ataques por segundo", as� que el tiempo entre ataques es 1 / attackSpeed
            tiempoSiguienteAtaque = 1f / playerStats.attackSpeed;
        }
    }

    private void Golpe()
    {
        anim.SetTrigger("Golpe");

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);
        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                // --- CAMBIO 4: Usamos el baseDamage de PlayerStats para el da�o ---
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