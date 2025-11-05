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

    // Estado interno de movimiento
    public enum MovementState { Stopped, Following, Fleeing }
    private MovementState currentState = MovementState.Following; // Empieza siguiendo

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
        if (jugador == null || currentState == MovementState.Stopped)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        Vector2 direccion;
        if (currentState == MovementState.Following)
        {
            direccion = (jugador.position - transform.position).normalized;
        }
        else // Fleeing (Huyendo)
        {
            direccion = (transform.position - jugador.position).normalized;
        }

        movimiento = direccion;

        // Asegúrate de que tus parámetros se llamen "MoveX" y "MoveY"
        animator.SetFloat("MoveX", movimiento.x);
        animator.SetFloat("MoveY", movimiento.y);
        animator.SetFloat("Speed", movimiento.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (jugador == null || currentState == MovementState.Stopped)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movimiento * velocidad;
    }

    // --- ¡MÉTODOS PÚBLICOS DE CONTROL! ---

    public void StopMovement()
    {
        currentState = MovementState.Stopped;
    }

    public void MoveTowards() // <-- El método que faltaba para el Tirador
    {
        currentState = MovementState.Following;
    }

    public void MoveAway() // <-- El método que faltaba para el Tirador
    {
        currentState = MovementState.Fleeing;
    }

    // Este método es para la MUERTE
    public void DesactivarIA()
    {
        StopMovement();
        enabled = false;
    }
}