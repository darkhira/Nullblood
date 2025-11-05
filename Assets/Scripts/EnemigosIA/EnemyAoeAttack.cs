using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyAoeAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject aoeZonePrefab; // El prefab de la zona de advertencia
    [SerializeField] private float attackRange = 8f; // Distancia para empezar a castear
    [SerializeField] private float attackCooldown = 4f;
    [SerializeField] private float aoeDamage = 15f;
    [SerializeField] private float castTime = 1.0f; // Tiempo que el enemigo se queda quieto casteando

    private Transform playerTarget;
    private Animator animator;
    private FollowIA followIA; // Referencia al script de movimiento
    private float attackTimer;
    private bool isCasting = false; // Para evitar que se mueva mientras castea

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        followIA = GetComponent<FollowIA>();
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (playerTarget == null || isCasting) return;

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. Si está en rango, DETIENE el movimiento
            followIA.StopMovement();

            // 2. Si el cooldown está listo, empieza a castear
            if (attackTimer <= 0)
            {
                StartCoroutine(CastAoe());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            // 3. Si está fuera de rango, REANUDA el movimiento
            // --- ¡CAMBIO REALIZADO! ---
            followIA.MoveTowards();
        }
    }

    private IEnumerator CastAoe()
    {
        isCasting = true;
        followIA.StopMovement();

        if (animator != null) animator.SetTrigger("Cast");

        yield return new WaitForSeconds(castTime);

        if (playerTarget != null)
        {
            GameObject zone = Instantiate(aoeZonePrefab, playerTarget.position, Quaternion.identity);
            if (zone.TryGetComponent<AoeZone>(out AoeZone aoeScript))
            {
                aoeScript.damage = this.aoeDamage;
            }
        }

        isCasting = false;
    }
}