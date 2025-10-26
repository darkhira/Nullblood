using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RangedEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2f;
    [Tooltip("Distancia mínima a la que deja de acercarse y empieza a disparar")]
    public float attackDistance = 6f;
    [Tooltip("Distancia máxima a la que detecta y sigue al jugador")]
    public float sightDistance = 12f;
    private Transform jugador;
    private Rigidbody2D rb;
    private Vector2 movimiento;
    private bool isFacingRight = true;

    [Header("Ataque a distancia")]
    public GameObject projectilePrefab;
    public Transform shootPoint; // hijo vacío donde salen las balas
    public float fireRate = 1f; // disparos por segundo
    public float projectileSpeed = 8f;
    public float projectileDamage = 10f;
    private float fireCooldown = 0f;

    [Header("Animaciones")]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
        else Debug.LogError("[RangedEnemy] No se encontró objeto con tag 'Player'.");
        if (shootPoint == null) Debug.LogWarning("[RangedEnemy] No se asignó shootPoint.");
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= sightDistance)
        {
            // dirección normalizada hacia el jugador
            Vector2 direccion = (jugador.position - transform.position).normalized;

            // Si está lejos del rango de ataque, acercarse
            if (distancia > attackDistance)
            {
                movimiento = direccion;
            }
            else // dentro del rango de ataque: detenerse y disparar
            {
                movimiento = Vector2.zero;
                TryAttack();
            }

            // Flip mirando al jugador
            bool movingRight = (jugador.position.x > transform.position.x);
            Flip(movingRight);
        }
        else
        {
            movimiento = Vector2.zero;
        }

        // Animación de movimiento
        animator.SetFloat("Speed", movimiento.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (movimiento != Vector2.zero)
        {
            rb.linearVelocity = movimiento * velocidad;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void TryAttack()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Disparar();
            fireCooldown = 1f / Mathf.Max(fireRate, 0.0001f);
        }
    }

    private void Disparar()
    {
        // Trigger animación (si la tienes)
        animator.SetTrigger("Attack");

        // Instanciar proyectil
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            // dirección hacia el jugador
            Vector2 dir = (jugador.position - shootPoint.position).normalized;
            projRb.linearVelocity = dir * projectileSpeed;
        }

        // Pasar parámetros al script Projectile (si existe)
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.damage = Mathf.RoundToInt(projectileDamage);
            p.ownerTag = gameObject.tag;
        }
    }

    private void Flip(bool movingRight)
    {
        if (isFacingRight != movingRight)
        {
            isFacingRight = movingRight;
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
    }

    // Método público para desactivar IA si el enemigo muere (coherente con tu FollowIA)
    public void DesactivarIA()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        enabled = false;
    }
}
