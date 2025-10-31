using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FollowIA))]
public class EnemyAoeAttack : MonoBehaviour
{
    [Header("Configuraci�n de Ataque")]
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
        if (playerTarget == null || isCasting) return; // No hace nada si no hay jugador o si est� casteando

        attackTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. Si est� en rango, DETIENE el movimiento
            followIA.StopMovement();

            // 2. Si el cooldown est� listo, empieza a castear
            if (attackTimer <= 0)
            {
                StartCoroutine(CastAoe());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            // 3. Si est� fuera de rango, REANUDA el movimiento
            followIA.ResumeMovement();
        }
    }

    private IEnumerator CastAoe()
    {
        isCasting = true;
        // (Ya deber�a estar detenido por el Update, pero lo aseguramos)
        followIA.StopMovement();

        // Inicia la animaci�n de casteo
        if (animator != null) animator.SetTrigger("Cast"); // Necesitar�s un Trigger "Cast"

        // Espera a que termine de castear
        yield return new WaitForSeconds(castTime);

        // Invoca la zona de AOE EN LA POSICI�N DEL JUGADOR
        if (playerTarget != null)
        {
            GameObject zone = Instantiate(aoeZonePrefab, playerTarget.position, Quaternion.identity);
            zone.GetComponent<AoeZone>().damage = this.aoeDamage;
        }

        isCasting = false;
        // No llamamos a ResumeMovement() aqu�, el Update() se encargar� de decidir
        // si debe seguir quieto (porque sigue en rango) o volver a moverse.
    }
}