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
    [SerializeField] private float attackPointOffset = 1.0f; // Asegúrate de tener esta variable

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
            followIA.StopMovement(); // Le dice al "conductor" que pare
            if (attackTimer <= 0)
            {
                StartCoroutine(AttackCoroutine());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            followIA.MoveTowards(); // Le dice al "conductor" que siga
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // El Animator ya sabe hacia dónde apuntar gracias al Update() de FollowIA
        if (animator != null) animator.SetTrigger("MeleeAttack");

        // 1. Obtiene la dirección actual del Animator
        Vector2 attackDirection = new Vector2(
            animator.GetFloat("MoveX"),
            animator.GetFloat("MoveY")
        ).normalized;

        // 2. Mueve el attackPoint a esa dirección
        if (attackPoint != null)
        {
            attackPoint.localPosition = attackDirection * attackPointOffset;
        }

        // Espera a que la animación conecte
        yield return new WaitForSeconds(0.3f);

        // 3. Aplica el daño
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        foreach (Collider2D playerCol in hitPlayers)
        {
            if (playerCol.CompareTag("Player"))
            {
                playerCol.GetComponent<PlayerStats>()?.TakeDamage(attackDamage, gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // No intentes dibujar si el punto de ataque no está asignado
        if (attackPoint == null)
        {
            return;
        }

        // Dibuja un círculo rojo en la posición del attackPoint con el radio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}