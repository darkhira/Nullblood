using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField] private int maxRicochets = 2;
    [SerializeField] private float ricochetRadius = 8f;
    [SerializeField] private int maxWallBounces = 1;

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
        rb.linearDamping = 0; // Antes drag
    }

    public void Throw(Transform owner, Vector2 direction)
    {
        playerTransform = owner;
        currentState = BoomerangState.Throwing;
        ricochetsLeft = maxRicochets;
        wallBouncesLeft = maxWallBounces;
        hitEnemies.Clear();

        rb.linearVelocity = direction.normalized * speed; // Antes velocity
        StartCoroutine(ReturnAfterTime(maxTravelTime));
    }

    void FixedUpdate()
    {
        if (currentState == BoomerangState.Returning && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * returnSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Lógica de retorno al jugador
        if (currentState == BoomerangState.Returning && other.transform == playerTransform)
        {
            HandlePlayerCatch(other);
            return;
        }

        // Evitar golpearse a sí mismo o al jugador mientras sale
        if (other.transform == playerTransform) return;

        // 2. Lógica de impacto contra Entidades (Enemigos o Boss)
        // Usamos un flag para saber si golpeamos algo válido
        bool hitValidTarget = false;

        // A) ¿Es un enemigo normal?
        if (other.TryGetComponent<Enemigo>(out Enemigo enemigo))
        {
            // Evitar golpear al mismo enemigo dos veces
            if (!hitEnemies.Contains(other.transform))
            {
                enemigo.TomarDaño(damage);
                hitEnemies.Add(other.transform);
                hitValidTarget = true;
            }
        }
        // B) ¿Es el Jefe Kopp?
        else if (other.TryGetComponent<BossKopp>(out BossKopp jefe))
        {
            if (!hitEnemies.Contains(other.transform))
            {
                jefe.TomarDaño(damage);
                hitEnemies.Add(other.transform);
                hitValidTarget = true;
            }
        }

        // Si golpeamos cualquiera de los dos, calculamos rebote
        if (hitValidTarget)
        {
            if (ricochetsLeft > 0)
            {
                ricochetsLeft--;
                Transform nextTarget = FindNextTarget();
                if (nextTarget != null)
                {
                    Vector2 direction = (nextTarget.position - transform.position).normalized;
                    rb.linearVelocity = direction * speed;
                }
                else
                {
                    StartReturning();
                }
            }
            else
            {
                StartReturning();
            }
        }
        // 3. Lógica de rebote en paredes
        else if (!other.isTrigger && currentState == BoomerangState.Throwing)
        {
            if (wallBouncesLeft > 0)
            {
                wallBouncesLeft--;
                // El rebote físico lo maneja el PhysicsMaterial2D si lo tienes, 
                // pero aquí aseguramos que no se detenga.
            }
            else
            {
                StartReturning();
            }
        }
    }

    private Transform FindNextTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ricochetRadius);

        // Buscamos objetos con Tag Enemigo que no hayamos golpeado ya
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
            StopAllCoroutines();
        }
    }

    private IEnumerator ReturnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StartReturning();
    }
}