using System;
using System.Collections; // Necesario para IEnumerator
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerSoundController playerSoundController;
    public static PlayerStats Instance;
    public static event Action OnStatsChanged;

    [Header("Configuración Base")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float baseDamage = 50f;
    public float attackSpeed = 1f;

    [Header("Invulnerabilidad")]
    [SerializeField] private float damageInvulnerabilityTime = 1f; // Tiempo de inmunidad tras golpe
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer; // Para el efecto visual

    private void Awake()
    {
        Debug.Log("<color=lime>--- PlayerStats HA DESPERTADO ---</color>");
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }

        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtenemos el sprite para hacerlo parpadear
    }

    private void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
    }

    private void Start()
    {
        OnStatsChanged?.Invoke();

        if (playerSoundController == null)
        {
            Debug.LogWarning("PlayerSoundController no está asignado en PlayerStats");
        }
    }

    // --- MÉTODO PARA CONTROLAR INVULNERABILIDAD EXTERNA (DASH) ---
    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;

        // Opcional: Si quieres que el dash también cambie el color del personaje
        // if(spriteRenderer != null) spriteRenderer.color = state ? new Color(1,1,1,0.5f) : Color.white;
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        // 1. SI ES INVULNERABLE, NO HACEMOS NADA
        if (isInvulnerable) return;

        Debug.LogError($"JUGADOR RECIBE {damage} DE DAÑO DESDE ---> {damageSource.name}");

        if (playerSoundController != null)
        {
            playerSoundController.playsonidoRecibirDanio();
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnStatsChanged?.Invoke();

        if (currentHealth <= 0)
        {
            playerSoundController.playsonidoRecibirDanio();
            Die();
        }
        else
        {
            // 2. SI SOBREVIVE, ACTIVAMOS LA INVULNERABILIDAD TEMPORAL
            StartCoroutine(InvulnerabilityCoroutine(damageInvulnerabilityTime));
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        OnStatsChanged?.Invoke();
    }

    public void ApplyCardEffect(CardSO card)
    {
        switch (card.effectType)
        {
            case CardSO.CardEffect.DamageIncrease:
                baseDamage += baseDamage * (card.effectValue / 100f);
                break;
            case CardSO.CardEffect.HealthIncrease:
                maxHealth += card.effectValue;
                currentHealth += card.effectValue;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                break;
            case CardSO.CardEffect.AttackSpeedIncrease:
                attackSpeed += attackSpeed * (card.effectValue / 100f);
                break;
        }
        OnStatsChanged?.Invoke();
    }

    private void Die()
    {
        if (playerSoundController != null)
        {
            playerSoundController.playsonidoMuerte();
        }
        Debug.Log("El jugador ha muerto.");
        gameObject.SetActive(false);
    }

    // --- CORRUTINA PARA PARPADEO E INMUNIDAD ---
    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;

        // Lógica simple de parpadeo
        float timer = 0;
        float blinkInterval = 0.1f;

        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                // Alterna entre visible y semitransparente
                spriteRenderer.enabled = !spriteRenderer.enabled;
                // O usa transparencia: 
                // var color = spriteRenderer.color;
                // color.a = (color.a == 1f) ? 0.5f : 1f;
                // spriteRenderer.color = color;
            }
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Restaurar estado original
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            // spriteRenderer.color = Color.white;
        }

        isInvulnerable = false;
    }
}