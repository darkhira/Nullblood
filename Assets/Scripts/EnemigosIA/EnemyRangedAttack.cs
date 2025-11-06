using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyRangedAttack : MonoBehaviour
{
    // ... (Todas tus variables de Configuración y Comportamiento)
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Comportamiento")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float attackCooldown = 2f;


    private Transform playerTarget;
    private Animator animator;
    private FollowIA followIA;
    private float attackTimer;

    // --- ¡NUEVA VARIABLE "SEGURO"! ---
    private bool hasFiredThisAttack = false;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        followIA = GetComponent<FollowIA>();
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (playerTarget == null) return;

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > attackRange)
        {
            followIA.MoveTowards();
        }
        else if (distanceToPlayer < minDistance)
        {
            followIA.MoveAway();
        }
        else
        {
            followIA.StopMovement();

            if (attackTimer <= 0)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
    }

    /// <summary>
    /// Inicia el proceso de ataque, resetea el "seguro" y activa la animación.
    /// </summary>
    private void Shoot()
    {
        // --- CAMBIO CLAVE: "Armamos" el disparo ---
        hasFiredThisAttack = false; // Permite que el próximo FireProjectile() funcione

        if (animator != null) animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Llamado por el Evento de Animación. Dispara la bala SÓLO UNA VEZ.
    /// </summary>
    public void FireProjectile()
    {
        // --- ¡EL "SEGURO" EN ACCIÓN! ---
        // Si ya hemos disparado en este ataque, ignoramos las llamadas duplicadas.
        if (hasFiredThisAttack) return;

        // Si no, ponemos el seguro.
        hasFiredThisAttack = true;
        // ---------------------------------

        Debug.LogWarning("+++ FireProjectile() llamado. ¡Disparando BALA ÚNICA! +++");

        if (playerTarget == null) return;

        Vector2 direction = (playerTarget.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile projectileScript = proj.GetComponent<EnemyProjectile>();

        if (projectileScript != null)
        {
            projectileScript.damage = this.projectileDamage;
            projectileScript.speed = this.projectileSpeed;
            proj.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        }
    }
}