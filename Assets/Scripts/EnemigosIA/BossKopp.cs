using UnityEngine;
using System.Collections;

public class BossKopp : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private string bossName = "Kopp, el Corruptor";
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;

    [Header("Estadísticas Base")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float contactDamage = 20f;

    [Header("Combate Melee (Golpes)")]
    [SerializeField] private float rangoAtaqueMelee = 1.5f; // Distancia para empezar a pegar
    [SerializeField] private float cooldownAtaque = 2f; // Tiempo entre golpes
    private float ultimoGolpeTime;

    [Header("Detección y Estado")]
    public bool estaActivo = false;
    [SerializeField] private float rangoDeteccion = 10f;
    private bool isDead = false;
    private bool isAttacking = false; // Bloquea movimiento mientras ataca

    [Header("Fases (Ácido)")]
    [SerializeField] private GameObject acidPoolPrefab;
    private bool phaseAcidActive = false;
    private bool phaseEnrageActive = false;
    [SerializeField] private float initialAcidInterval = 4f;
    private float currentAcidInterval;
    private float acidTimer;

    void Start()
    {
        currentHealth = maxHealth;
        currentAcidInterval = initialAcidInterval;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (BossHealthUI.Instance != null) BossHealthUI.Instance.DeactivateBossHealthBar();
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        // 1. ACTIVACIÓN (Si está dormido)
        if (!estaActivo)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist <= rangoDeteccion) ActivarCombate();
            return;
        }

        // Calculamos distancia y dirección
        float distanciaJugador = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // 2. ACTUALIZAR ANIMATOR (Dirección de la mirada)
        if (animator != null)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            // Si ataca, Speed es 0 (quieto), si no, se mueve
            animator.SetFloat("Speed", isAttacking ? 0f : 1f);
        }

        // 3. LÓGICA DE COMBATE
        if (!isAttacking)
        {
            // A) ATAQUE MELEE: Si está cerca y pasó el tiempo de cooldown
            if (distanciaJugador <= rangoAtaqueMelee && Time.time >= ultimoGolpeTime + cooldownAtaque)
            {
                StartCoroutine(PerformMeleeAttack(direction));
            }
            // B) PERSECUCIÓN: Si no está atacando y está lejos, se mueve
            else if (distanciaJugador > rangoAtaqueMelee)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }
        }

        // 4. FASES Y ÁCIDO (Independiente del melee, sigue tirando ácido si toca)
        if (phaseAcidActive && !isAttacking)
        {
            acidTimer += Time.deltaTime;
            if (acidTimer >= currentAcidInterval)
            {
                // Usamos la misma animación de ataque para lanzar ácido (o podrías crear otra)
                StartCoroutine(PerformAcidAttack(direction));
                acidTimer = 0f;
            }
        }
    }

    private void ActivarCombate()
    {
        estaActivo = true;
        if (BossHealthUI.Instance != null) BossHealthUI.Instance.ActivateBossHealthBar(bossName, maxHealth);
        if (animator != null) animator.SetTrigger("Enrage");
    }

    // --- CORRUTINA DE ATAQUE CUERPO A CUERPO ---
    private IEnumerator PerformMeleeAttack(Vector2 attackDir)
    {
        isAttacking = true; // Detiene el movimiento
        ultimoGolpeTime = Time.time; // Reinicia cooldown

        if (animator != null) animator.SetTrigger("Attack"); // Animación de golpe

        // Esperamos el momento del impacto (ajusta esto según tu animación)
        // Si el puñetazo sale al segundo 0.3 de la animación, pon 0.3f
        yield return new WaitForSeconds(0.3f);

        // Verificamos si el jugador sigue cerca para recibir el daño
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoAtaqueMelee + 0.5f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Le hacemos daño (usamos contactDamage o un daño específico de golpe)
                hit.GetComponent<PlayerStats>()?.TakeDamage(contactDamage, gameObject);
            }
        }

        // Esperamos a que termine la animación
        yield return new WaitForSeconds(0.4f);
        isAttacking = false; // Vuelve a moverse
    }

    // --- CORRUTINA DE ATAQUE DE ÁCIDO (A distancia) ---
    private IEnumerator PerformAcidAttack(Vector2 attackDir)
    {
        isAttacking = true;
        if (animator != null) animator.SetTrigger("Attack"); // Reusamos animación o usa otra

        yield return new WaitForSeconds(0.4f);
        if (acidPoolPrefab) Instantiate(acidPoolPrefab, playerTransform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }

    public void TomarDaño(float amount)
    {
        if (isDead) return;
        if (!estaActivo) ActivarCombate();

        currentHealth -= amount;
        if (BossHealthUI.Instance != null) BossHealthUI.Instance.UpdateHealth(currentHealth);

        // Solo hacemos animación de Hit si NO está atacando (para no cancelar el ataque)
        if (!isAttacking && animator != null) animator.SetTrigger("Hit");

        if (currentHealth <= 0) { Die(); return; }

        CheckPhases();
    }

    private void CheckPhases()
    {
        float pct = currentHealth / maxHealth;
        if (pct <= 0.75f && !phaseAcidActive) { phaseAcidActive = true; spriteRenderer.color = new Color(0.8f, 1f, 0.8f); }
        if (pct <= 0.50f && !phaseEnrageActive) ActivateEnragePhase();
    }

    private void ActivateEnragePhase()
    {
        phaseEnrageActive = true;
        if (animator != null) animator.SetTrigger("Enrage");

        transform.localScale *= 1.5f;
        contactDamage *= 1.5f;
        currentAcidInterval /= 2f;
        cooldownAtaque /= 1.5f; // <-- ENRAGE: Pega más rápido ahora

        spriteRenderer.color = Color.red;
    }

    private void Die()
    {
        isDead = true;
        if (animator != null) animator.SetBool("Die", true);
        if (BossHealthUI.Instance != null) BossHealthUI.Instance.DeactivateBossHealthBar();
        Destroy(gameObject, 0.8f);
    }

    // Daño por contacto pasivo (si lo tocas sin que ataque)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerStats>()?.TakeDamage(contactDamage, gameObject);
    }

    // DIBUJAR GIZMOS PARA VER LOS RANGOS
    private void OnDrawGizmosSelected()
    {
        // Rango de visión (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        // Rango de Ataque Melee (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaqueMelee);
    }
}