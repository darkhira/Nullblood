using UnityEngine;

[RequireComponent(typeof(FollowIA))] // Asegura que este script tenga un FollowIA
public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackRange = 10f; // Distancia para empezar a disparar
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileSpeed = 12f;

    private Transform playerTarget;
    private Animator animator;
    private FollowIA followIA; // --- NUEVA REFERENCIA ---
    private float attackTimer;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        followIA = GetComponent<FollowIA>(); // --- OBTENEMOS LA REFERENCIA ---
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (playerTarget == null) return;

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. Si está en rango, DETIENE el movimiento
            followIA.StopMovement();

            // 2. Si el cooldown está listo, dispara
            if (attackTimer <= 0)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            // 3. Si está fuera de rango, REANUDA el movimiento
            followIA.ResumeMovement();
        }
    }

    private void Shoot()
    {
        if (animator != null) animator.SetTrigger("Attack");

        Vector2 direction = (playerTarget.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile projectileScript = proj.GetComponent<EnemyProjectile>();

        projectileScript.damage = this.projectileDamage;
        projectileScript.speed = this.projectileSpeed;

        proj.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
    }
}