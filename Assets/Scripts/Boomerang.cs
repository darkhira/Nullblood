using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para usar OrderBy y listas avanzadas

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
    [SerializeField] private int maxWallBounces = 3; // Máximo rebotes en paredes

    // Estado interno
    private enum BoomerangState { Throwing, Returning }
    private BoomerangState currentState;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private int ricochetsLeft;
    private int wallBouncesLeft;
    private List<Transform> hitEnemies = new List<Transform>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        // Nota: En versiones nuevas de Unity es linearDamping, en viejas es drag.
        // Si te da error aquí, cámbialo por: rb.drag = 0;
        rb.linearDamping = 0;
    }

    public void Throw(Transform owner, Vector2 direction)
    {
        playerTransform = owner;
        currentState = BoomerangState.Throwing;
        ricochetsLeft = maxRicochets;
        wallBouncesLeft = maxWallBounces;
        hitEnemies.Clear();

        rb.linearVelocity = direction.normalized * speed; // velocity en versiones viejas
        StartCoroutine(ReturnAfterTime(maxTravelTime));
    }

    void FixedUpdate()
    {
        // Si está en modo retorno, persigue al jugador
        if (currentState == BoomerangState.Returning && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * returnSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. LOGICA DE ATRAPAR (Retorno al jugador)
        if (currentState == BoomerangState.Returning && other.transform == playerTransform)
        {
            HandlePlayerCatch(other);
            return;
        }

        // Evitar chocar con el jugador mientras sale disparado
        if (other.transform == playerTransform) return;

        // 2. LOGICA DE IMPACTO CON ENTIDADES (Enemigo o Jefe)
        bool hitValidTarget = false;

        // Intentar obtener componente Enemigo (Mobs normales)
        if (other.TryGetComponent<Enemigo>(out Enemigo enemigo))
        {
            if (!hitEnemies.Contains(other.transform))
            {
                enemigo.TomarDaño(damage);
                hitEnemies.Add(other.transform);
                hitValidTarget = true;
            }
        }
        // Intentar obtener componente BossKopp (El Jefe)
        else if (other.TryGetComponent<BossKopp>(out BossKopp jefe))
        {
            if (!hitEnemies.Contains(other.transform))
            {
                jefe.TomarDaño(damage);
                hitEnemies.Add(other.transform);
                hitValidTarget = true;
            }
        }

        // Si golpeamos a alguien válido...
        if (hitValidTarget)
        {
            if (ricochetsLeft > 0)
            {
                ricochetsLeft--;
                Transform nextTarget = FindNextTarget();

                if (nextTarget != null)
                {
                    // Apuntar al siguiente enemigo
                    Vector2 direction = (nextTarget.position - transform.position).normalized;
                    rb.linearVelocity = direction * speed;
                }
                else
                {
                    // Si no hay nadie más cerca, volver.
                    StartReturning();
                }
            }
            else
            {
                // Se acabaron los rebotes
                StartReturning();
            }
        }
        // 3. LOGICA DE REBOTE EN PAREDES (Por Tag)
        // Verificamos si el objeto tiene el Tag "Paredes" y estamos lanzando
        else if (other.CompareTag("Paredes") && currentState == BoomerangState.Throwing)
        {
            HandleWallBounce(other);
        }
    }

    /// <summary>
    /// Calcula el rebote físico usando un Raycast inverso porque el objeto es un Trigger.
    /// </summary>
    private void HandleWallBounce(Collider2D wallCollider)
    {
        if (wallBouncesLeft > 0)
        {
            wallBouncesLeft--;

            // TÉCNICA DEL RAYCAST INVERSO
            // Como somos un Trigger, ya entramos en la pared.
            // Lanzamos un rayo hacia ATRÁS (desde donde veníamos) para encontrar el punto de entrada y la normal.

            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 backDirection = -currentVelocity.normalized;

            // Buscamos colisión en Default o Paredes (ajusta la máscara según tus Layers si es necesario)
            // El rayo sale desde el centro del bumerán hacia atrás.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, backDirection, 2f, LayerMask.GetMask("Default", "Paredes", "Obstaculos"));

            // NOTA: Si tus paredes están en la capa "Default", esto funcionará. 
            // Si están en una capa propia, añade el nombre en GetMask.

            if (hit.collider != null)
            {
                // Calculamos el vector de rebote matemático
                Vector2 newDirection = Vector2.Reflect(currentVelocity.normalized, hit.normal);

                // Aplicamos la nueva velocidad
                rb.linearVelocity = newDirection * speed;
            }
            else
            {
                // Si el Raycast falla (ej. esquina muy fina), intentamos volver para no quedarnos atascados
                StartReturning();
            }
        }
        else
        {
            // Se acabaron los rebotes de pared
            StartReturning();
        }
    }

    private Transform FindNextTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ricochetRadius);

        var potentialTargets = hits
            .Where(hit => (hit.CompareTag("Enemigo") || hit.GetComponent<BossKopp>() != null) && !hitEnemies.Contains(hit.transform))
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
            StopAllCoroutines();
        }
    }

    private IEnumerator ReturnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StartReturning();
    }
}