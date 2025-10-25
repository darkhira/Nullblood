using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para OrderBy

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Boomerang : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float returnSpeed = 12f;
    [SerializeField] private float maxTravelTime = 2f;
    [SerializeField] private float damage = 10f;

    [Header("Rebotes")]
    [SerializeField] private int maxRicochets = 2; // Máximo rebotes entre enemigos
    [SerializeField] private float ricochetRadius = 8f; // Radio para buscar sig. enemigo
    [SerializeField] private int maxWallBounces = 1; // Máximo rebotes en paredes

    // Estado interno
    private enum BoomerangState { Throwing, Returning }
    private BoomerangState currentState;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private int ricochetsLeft;
    private List<Transform> hitEnemies = new List<Transform>();
    private int wallBouncesLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.linearDamping = 0;
    }

    public void Throw(Transform thrower, Vector2 direction)
    {
        playerTransform = thrower;
        ricochetsLeft = maxRicochets;
        wallBouncesLeft = maxWallBounces; // Inicializa rebotes de pared
        hitEnemies.Clear();
        currentState = BoomerangState.Throwing;
        rb.linearVelocity = direction.normalized * speed;
        StartCoroutine(ReturnAfterTime(maxTravelTime));
    }

    void FixedUpdate()
    {
        if (currentState == BoomerangState.Returning && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * returnSpeed;
        }
        transform.Rotate(0, 0, 720 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo procesamos colisiones si está en estado "Throwing"
        if (currentState == BoomerangState.Throwing)
        {
            // Prioridad 1: Paredes
            if (other.CompareTag("Paredes"))
            {
                HandleWallBounce(other);
            }
            // Prioridad 2: Enemigos
            else if (other.CompareTag("Enemigo"))
            {
                HandleEnemyHit(other);
            }
        }
        // Lógica para atrapar el bumerán (cuando está "Returning")
        else if (currentState == BoomerangState.Returning && other.CompareTag("Player"))
        {
            HandlePlayerCatch(other);
        }
    }

    /// <summary>
    /// Gestiona el rebote contra una pared.
    /// </summary>
    private void HandleWallBounce(Collider2D wallCollider)
    {
        if (wallBouncesLeft <= 0)
        {
            StartReturning(); // Si no quedan rebotes de pared, vuelve
            return;
        }

        StopAllCoroutines(); // Detiene el temporizador de retorno por tiempo

        // Intenta obtener la normal precisa usando Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position - (Vector3)rb.linearVelocity.normalized * 0.1f, rb.linearVelocity.normalized, 0.2f, LayerMask.GetMask("Default")); // Ajusta LayerMask si es necesario

        Vector2 hitNormal;
        if (hit.collider != null)
        {
            hitNormal = hit.normal;
        }
        else
        {
            // Aproximación si el Raycast falla
            hitNormal = (transform.position - wallCollider.bounds.ClosestPoint(transform.position)).normalized;
        }

        // Refleja la velocidad
        Vector2 reflectedVelocity = Vector2.Reflect(rb.linearVelocity, hitNormal);
        rb.linearVelocity = reflectedVelocity.normalized * speed; // Mantiene la velocidad
        wallBouncesLeft--;

        // Reinicia el temporizador de retorno
        StartCoroutine(ReturnAfterTime(maxTravelTime));
    }

    /// <summary>
    /// Gestiona el golpe a un enemigo y decide si rebotar o volver.
    /// </summary>
    private void HandleEnemyHit(Collider2D enemyCollider)
    {
        // Ignora si ya golpeó a este enemigo
        if (hitEnemies.Contains(enemyCollider.transform)) return;

        StopAllCoroutines(); // Detiene el temporizador de retorno por tiempo

        // Registra, daña y reduce rebotes de enemigo
        hitEnemies.Add(enemyCollider.transform);
        enemyCollider.GetComponent<Enemigo>()?.TomarDaño(damage);
        ricochetsLeft--;

        // Comprueba si quedan rebotes de enemigo
        if (ricochetsLeft >= 0)
        {
            Transform nextTarget = FindNextTarget(); // Busca el siguiente objetivo
            if (nextTarget != null)
            {
                // Rebota hacia el siguiente objetivo
                Vector2 direction = (nextTarget.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
                // Reinicia el temporizador de retorno
                StartCoroutine(ReturnAfterTime(maxTravelTime));
            }
            else
            {
                // No encontró otro enemigo cercano, empieza a volver
                StartReturning();
            }
        }
        else
        {
            // Se acabaron los rebotes de enemigo permitidos, empieza a volver
            StartReturning();
        }
    }

    /// <summary>
    /// Busca el enemigo más cercano dentro del radio que no haya sido golpeado aún.
    /// </summary>
    private Transform FindNextTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ricochetRadius);
        var potentialTargets = hits
            .Where(hit => hit.CompareTag("Enemigo") && !hitEnemies.Contains(hit.transform))
            .OrderBy(hit => Vector2.Distance(transform.position, hit.transform.position));
        return potentialTargets.FirstOrDefault()?.transform;
    }


    private void HandlePlayerCatch(Collider2D playerCollider)
    {
        PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.ResetBoomerangCooldown();
        }
        Destroy(gameObject);
    }

    private void StartReturning()
    {
        if (currentState == BoomerangState.Throwing)
        {
            currentState = BoomerangState.Returning;
            StopAllCoroutines(); // Detiene cualquier temporizador pendiente
        }
    }

    private IEnumerator ReturnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        // Si después del tiempo sigue en modo "Throwing", forzamos el retorno
        StartReturning();
    }
}