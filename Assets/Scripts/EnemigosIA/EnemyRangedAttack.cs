using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Comportamiento")]
    [Tooltip("Distancia máxima a la que el enemigo intentará disparar.")]
    [SerializeField] private float attackRange = 10f;
    [Tooltip("Distancia mínima que el enemigo intentará mantener. Si el jugador se acerca más, retrocederá.")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float attackCooldown = 2f;

    private Transform playerTarget;
    private Animator animator;
    private FollowIA followIA; // Referencia al "conductor"
    private float attackTimer;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        followIA = GetComponent<FollowIA>(); // Obtenemos la referencia
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (playerTarget == null) return;

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // --- LÓGICA DE DECISIÓN ---

        if (distanceToPlayer > attackRange)
        {
            // 1. Demasiado lejos: Perseguir
            followIA.MoveTowards();
        }
        else if (distanceToPlayer < minDistance)
        {
            // 2. Demasiado cerca: Retroceder
            followIA.MoveAway();
        }
        else
        {
            // 3. ¡Distancia perfecta! Detenerse y disparar.
            followIA.StopMovement();

            if (attackTimer <= 0)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
    }

    private void Shoot()
    {
        if (animator != null) animator.SetTrigger("Attack");

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