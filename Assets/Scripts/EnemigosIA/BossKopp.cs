using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossKopp : MonoBehaviour
{
    public PlayerSoundController playerSoundController;
    [Header("Detecci�n del Jugador")]
    [SerializeField] private float rangoDeteccion = 10f; // Distancia a la que se activa
    public bool estaActivo = false; // Empieza dormido

    [Header("Estad�sticas Base")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float contactDamage = 20f;
    [SerializeField] private string bossName = "Kopp, el Corruptor";

    [Header("Fases")]
    [SerializeField] private GameObject acidPoolPrefab;
    private bool phaseAcidActive = false;
    private bool phaseEnrageActive = false;
    [SerializeField] private float initialAcidInterval = 4f;
    private float currentAcidInterval;
    private float acidTimer;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        currentAcidInterval = initialAcidInterval;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        // Asegurarnos que la UI est� apagada al nacer el jefe
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.DeactivateBossHealthBar();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // --- L�GICA DE ACTIVACI�N POR RANGO ---
        if (!estaActivo)
        {
            // Calculamos la distancia entre el Jefe y el Jugador
            float distancia = Vector2.Distance(transform.position, playerTransform.position);

            // Si entra en el rango... �DESPERTAR!
            if (distancia <= rangoDeteccion)
            {
                ActivarCombate();
            }
            return; // Mientras no est� activo, no hace nada m�s en el Update
        }
        // ---------------------------------------

        // L�gica de persecuci�n
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

        // L�gica de fases
        if (phaseAcidActive)
        {
            acidTimer += Time.deltaTime;
            if (acidTimer >= currentAcidInterval)
            {
                if (acidPoolPrefab) Instantiate(acidPoolPrefab, playerTransform.position, Quaternion.identity);
                acidTimer = 0f;
            }
        }
    }

    private void ActivarCombate()
    {
        estaActivo = true;
        Debug.Log("�JUGADOR EN RANGO! KOPP HA DESPERTADO.");

        // Activamos la UI usando el Singleton
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.ActivateBossHealthBar(bossName, maxHealth);
        }
    }

    public void TomarDaño(float amount)
    {
        // Si le pegan desde lejos, se despierta autom�ticamente
        if (!estaActivo) ActivarCombate();

        currentHealth -= amount;

        // Actualizar UI
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0) Die();

        float healthPercentage = currentHealth / maxHealth;
        if (healthPercentage <= 0.75f && !phaseAcidActive) { phaseAcidActive = true; if (spriteRenderer) spriteRenderer.color = new Color(0.8f, 1f, 0.8f); }
        if (healthPercentage <= 0.50f && !phaseEnrageActive) { phaseEnrageActive = true; transform.localScale *= 1.5f; contactDamage *= 1.5f; currentAcidInterval /= 2f; if (spriteRenderer) spriteRenderer.color = Color.red; }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>()?.TakeDamage(contactDamage, gameObject);
        }
    }

    private void Die()
    {
        if (BossHealthUI.Instance != null) BossHealthUI.Instance.DeactivateBossHealthBar();
        var playerSoundController = FindObjectOfType<PlayerSoundController>();
    if (playerSoundController != null)
    {
        playerSoundController.playsonidoMuerteKopp(); // Si "sonidoMuerteVorr" es el sonido de Kopp
        // ...o usa el método que realmente corresponde (puedes crear uno exclusivo si lo prefieres):
        // playerSoundController.playsonidoMuerteKopp();
    }
         if (MusicManagerGlobal.instance != null)
    {
        MusicManagerGlobal.instance.GetComponent<AudioSource>().Stop();
        // O bien: MusicManagerGlobal.instance.audioSource.Stop(); si lo tienes público.
    }
        SceneManager.LoadScene("Outro");
        Destroy(gameObject);
    }

    // --- DIBUJAR EL RANGO EN EL EDITOR (GIZMOS) ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}