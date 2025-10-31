using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Configuraci�n de Ataque")]
    [SerializeField] private float attackRange = 1.5f; // Distancia para empezar a golpear
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private Transform attackPoint; // Un objeto hijo que marca de d�nde sale el golpe

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
            // 1. Si est� en rango, DETIENE el movimiento
            followIA.StopMovement();

            // 2. Si el cooldown est� listo, ataca
            if (attackTimer <= 0)
            {
                StartCoroutine(AttackCoroutine());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            // 3. Si est� fuera de rango, REANUDA el movimiento
            followIA.ResumeMovement();
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // Activa la animaci�n de ataque
        if (animator != null) animator.SetTrigger("Attack"); // Necesitar�s un Trigger "Attack"

        // Espera un momento para que la animaci�n se vea antes de aplicar el da�o
        yield return new WaitForSeconds(0.3f);

        // Aplica el da�o
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