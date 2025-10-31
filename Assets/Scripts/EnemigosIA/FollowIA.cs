using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class FollowIA : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private Transform jugador;
    private Rigidbody2D rb;

    [Header("Animaciones")]
    private Animator animator;
    private Vector2 movimiento;

    // --- NUEVA VARIABLE DE ESTADO ---
    // Otros scripts (como los de ataque) pondrán esto en 'true' para detener al enemigo.
    private bool isStopped = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            jugador = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Player'. La IA no se moverá.");
            isStopped = true; // Detiene la IA si no hay jugador
        }
    }

    void Update()
    {
        if (jugador == null || isStopped)
        {
            // Si está detenido o no hay jugador, nos aseguramos de que la animación esté en 'Idle'.
            animator.SetFloat("Speed", 0);
            return;
        }

        // 1. Calcular dirección hacia el jugador
        Vector2 direccion = (jugador.position - transform.position).normalized;
        movimiento = direccion;

        // 2. Actualizar animaciones (Blend Tree)
        // (Nota: Tu Blend Tree usa "MoveX" y "MoveY", lo cual es un poco inusual.
        // Lo estándar es "Horizontal" y "Vertical", pero respetaré tus nombres)
        animator.SetFloat("MoveX", movimiento.x);
        animator.SetFloat("MoveY", movimiento.y);
        animator.SetFloat("Speed", movimiento.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (jugador == null || isStopped)
        {
            rb.linearVelocity = Vector2.zero; // Asegura que el Rigidbody esté quieto
            return;
        }

        // Aplicar movimiento
        rb.linearVelocity = movimiento * velocidad;
    }

    // --- NUEVOS MÉTODOS PÚBLICOS ---
    public void StopMovement()
    {
        isStopped = true;
    }

    public void ResumeMovement()
    {
        isStopped = false;
    }

    // Este método es para la MUERTE, no para los ataques. Lo dejamos como estaba.
    public void DesactivarIA()
    {
        isStopped = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        enabled = false;
    }
}