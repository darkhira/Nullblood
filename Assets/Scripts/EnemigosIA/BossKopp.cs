using UnityEngine;
using System.Collections;

public class BossKopp : MonoBehaviour
{
    [Header("UI del Jefe")]
    [SerializeField] private BossHealthUI bossHealthUI; // <--- ARRASTRA AQUÍ EL SCRIPT DE LA UI
    [SerializeField] private string bossName = "Kopp, el Corruptor";

    [Header("Estadísticas Base")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float contactDamage = 20f;

    [Header("Configuración de Fases")]
    [SerializeField] private GameObject acidPoolPrefab;
    [SerializeField] private Transform attackPoint;

    private bool phaseAcidActive = false;
    private bool phaseEnrageActive = false;

    [Header("Tiempos de Ácido")]
    [SerializeField] private float initialAcidInterval = 4f;
    private float currentAcidInterval;
    private float acidTimer;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        currentAcidInterval = initialAcidInterval;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // --- NUEVO BLOQUE: BUSCADOR AUTOMÁTICO ---
        // Si se te olvidó arrastrar la UI, el código la busca solo.
        if (bossHealthUI == null)
        {
            bossHealthUI = FindObjectOfType<BossHealthUI>();
        }
        // ------------------------------------------

        if (bossHealthUI != null)
        {
            bossHealthUI.ActivateBossHealthBar(bossName, maxHealth);
        }
        else
        {
            Debug.LogWarning("¡El Jefe no encontró el script BossHealthUI en la escena!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

        if (phaseAcidActive)
        {
            HandleAcidSpawns();
        }
    }

    public void TomarDaño(float amount)
    {
        currentHealth -= amount;

        // --- ACTUALIZAR BARRA DE VIDA ---
        if (bossHealthUI != null)
        {
            bossHealthUI.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage <= 0.75f && !phaseAcidActive) ActivateAcidPhase();
        if (healthPercentage <= 0.50f && !phaseEnrageActive) ActivateEnragePhase();
    }

    // ... (Los métodos ActivateAcidPhase, ActivateEnragePhase, HandleAcidSpawns y SpawnAcid siguen igual) ...
    // Copia esos métodos del script anterior si no los tienes a mano.

    // Solo repito los métodos de fases brevemente para que compile si copias todo:
    private void ActivateAcidPhase() { phaseAcidActive = true; if (spriteRenderer) spriteRenderer.color = new Color(0.8f, 1f, 0.8f); }
    private void ActivateEnragePhase() { phaseEnrageActive = true; transform.localScale *= 1.5f; contactDamage *= 1.5f; currentAcidInterval /= 2f; if (spriteRenderer) spriteRenderer.color = Color.red; }
    private void HandleAcidSpawns() { acidTimer += Time.deltaTime; if (acidTimer >= currentAcidInterval) { SpawnAcid(); acidTimer = 0f; } }
    private void SpawnAcid() { if (acidPoolPrefab) Instantiate(acidPoolPrefab, playerTransform.position, Quaternion.identity); }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
            if (stats != null) stats.TakeDamage(contactDamage, gameObject);
        }
    }

    private void Die()
    {
        Debug.Log("Kopp ha sido derrotado.");

        // --- OCULTAR BARRA AL MORIR ---
        if (bossHealthUI != null)
        {
            bossHealthUI.DeactivateBossHealthBar();
        }

        Destroy(gameObject);
    }
}