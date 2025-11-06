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

    public enum MovementState { Stopped, Following, Fleeing }
    private MovementState currentState = MovementState.Following;

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
            currentState = MovementState.Stopped;
        }
    }

    void Update()
    {
        if (jugador == null)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        // --- ¡LÓGICA CLAVE! ---
        // 1. SIEMPRE calculamos la dirección para APUNTAR.
        Vector2 direccion = (jugador.position - transform.position).normalized;

        // 2. SIEMPRE actualizamos los parámetros del Animator.
        // Esto permite que el Blend Tree de "Attack" sepa hacia dónde apuntar,
        // incluso si el enemigo está quieto.
        animator.SetFloat("MoveX", direccion.x);
        animator.SetFloat("MoveY", direccion.y);
        // -------------------------

        // 3. Decidimos el movimiento basándonos en el estado
        if (currentState == MovementState.Stopped)
        {
            movimiento = Vector2.zero;
        }
        else if (currentState == MovementState.Following)
        {
            movimiento = direccion;
        }
        else // Fleeing
        {
            movimiento = -direccion;
        }

        // 4. Actualizamos el 'Speed' para las animaciones de Idle/Walk
        animator.SetFloat("Speed", movimiento.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (jugador == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Aplicamos el movimiento (será Vector2.zero si estamos en estado Stopped)
        rb.linearVelocity = movimiento * velocidad;
    }

    // --- Métodos de Control (no cambian) ---
    public void StopMovement() { currentState = MovementState.Stopped; }
    public void MoveTowards() { currentState = MovementState.Following; }
    public void MoveAway() { currentState = MovementState.Fleeing; }
    public void DesactivarIA() { StopMovement(); enabled = false; }
}