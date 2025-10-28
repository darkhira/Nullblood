using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RangedEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float rangoAtaque = 6f;
    [SerializeField] private float rangoDeteccion = 16f;

    public Transform Player;
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
            Player = playerObj.transform;
        else
            Debug.LogError("[RangedEnemy] No se encontró un objeto con tag 'Player'.");

        if (torreta == null)
            Debug.LogWarning("[RangedEnemy] No se asignó la referencia a la torreta.");
    }

    void Update()
    {
        if (Player == null) return;
        // Evita que la torreta se invierta visualmente al hacer flip
        if (torreta != null)
        {
            Vector3 scale = torreta.transform.localScale;
            scale.x = Mathf.Abs(scale.x); // siempre positivo
            torreta.transform.localScale = scale;
        }
        float distancia = Vector2.Distance(transform.position, Player.position);

        if (distancia <= rangoDeteccion)
        {
            // Si está más lejos del rango de ataque, moverse hacia el jugador
            if (distancia > rangoAtaque)
            {
                Vector2 direccion = (Player.position - transform.position).normalized;
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
            bool movingRight = (Player.position.x > transform.position.x);
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
    /*
    private void FixedUpdate()
    {
        if (!enabled || Player == null) return;

        // Dirección al jugador
        Vector2 toPlayer = Player.position - transform.position;
        float distance = toPlayer.magnitude;
        Vector2 dir = toPlayer.normalized;

        Vector2 desiredVelocity = Vector2.zero;

        // Si está lejos, se acerca; si está muy cerca, se aleja
        if (distance > rangoAtaque)
            desiredVelocity = dir * velocidad;
        else if (distance < retroceso)
            desiredVelocity = -dir * velocidad;

        // Movimiento suave
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
    }*/

    private void Flip(bool movingRight)
{
        if (isFacingRight != movingRight)
        {
            isFacingRight = movingRight;

            // Invertir todo el enemigo
            Vector3 scale = transform.localScale;
            scale.x *= 1;
            transform.localScale = scale;

        }
        else
        {
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
