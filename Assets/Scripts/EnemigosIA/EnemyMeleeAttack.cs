using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private Transform attackPoint;

    private Transform playerTarget;
    private FollowIA followIA;
    private Animator animator;
    private float attackTimer;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        followIA = GetComponent<FollowIA>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerTarget == null) return;

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            followIA.StopMovement();

            if (attackTimer <= 0)
            {
                StartCoroutine(AttackCoroutine());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            // --- CAMBIO DE NOMBRE: De ResumeMovement a MoveTowards ---
            followIA.MoveTowards();
        }
    }

    private IEnumerator AttackCoroutine()
    {
        if (animator != null) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        foreach (Collider2D playerCol in hitPlayers)
        {
            if (playerCol.CompareTag("Player"))
            {
                playerCol.GetComponent<PlayerStats>()?.TakeDamage(attackDamage, gameObject);
            }
        }
    }
}