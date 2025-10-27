using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RangedEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float rangoAtaque = 6f;
    [SerializeField] private float rangoDeteccion = 24f;

    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isFacingRight = true;
    private bool puedeDisparar = false;

    [Header("Referencia a la torreta")]
    [SerializeField] private RangedTurret torreta; // Asigna el objeto Torreta en el inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
        else
            Debug.LogError("[RangedEnemy] No se encontró un objeto con tag 'Player'.");

        if (torreta == null)
            Debug.LogWarning("[RangedEnemy] No se asignó la referencia a la torreta.");
    }

    void Update()
    {
        if (jugador == null) return;
        // Evita que la torreta se invierta visualmente al hacer flip
    if (torreta != null)
    {
        Vector3 scale = torreta.transform.localScale;
        scale.x = Mathf.Abs(scale.x); // siempre positivo
        torreta.transform.localScale = scale;
    }
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion)
        {
            // Si está más lejos del rango de ataque, moverse hacia el jugador
            if (distancia > rangoAtaque)
            {
                Vector2 direccion = (jugador.position - transform.position).normalized;
                rb.linearVelocity = direccion * velocidad;

                // Mientras se mueve, desactiva el disparo
                puedeDisparar = false;
                if (torreta != null) torreta.SetPuedeDisparar(false);
            }
            else
            {
                // Detenerse y activar disparo
                rb.linearVelocity = Vector2.zero;
                puedeDisparar = true;
                if (torreta != null) torreta.SetPuedeDisparar(true);
            }

            // Flip horizontal
            bool movingRight = (jugador.position.x > transform.position.x);
            Flip(movingRight);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            puedeDisparar = false;
            if (torreta != null) torreta.SetPuedeDisparar(false);
        }

        // Animación
        animator.SetFloat("Speed", rb.linearVelocity.sqrMagnitude);
    }

    private void Flip(bool movingRight)
{
    if (isFacingRight != movingRight)
    {
        isFacingRight = movingRight;

        // Invertir todo el enemigo
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }
}


    // Llamado por Enemigo cuando muere
    public void DesactivarIA()
    {
        rb.linearVelocity = Vector2.zero;
        enabled = false;
        if (torreta != null)
            torreta.SetPuedeDisparar(false);
    }
}
