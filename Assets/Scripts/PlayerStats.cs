// Versi�n ligeramente mejorada de PlayerStats.cs
using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // ... (Instance, evento y variables como antes) ...
    public PlayerSoundController playerSoundController;
    public static PlayerStats Instance;
    public static event Action OnStatsChanged;
    public float maxHealth = 100f, currentHealth, baseDamage = 50f, attackSpeed = 1f;




    private void Awake()
    {
        Debug.Log("<color=lime>--- PlayerStats HA DESPERTADO ---</color>");
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        currentHealth = maxHealth;

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

    // --- M�TODOS P�BLICOS ---
    public void TakeDamage(float damage, GameObject damageSource)
    {
        Debug.LogError($"JUGADOR RECIBE {damage} DE DA�O DESDE ---> {damageSource.name}");

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
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        OnStatsChanged?.Invoke(); // Lanza el evento una vez
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
                // La curaci�n ya no necesita llamar al evento, ApplyCardEffect lo har�.
                currentHealth += card.effectValue;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                break;
            case CardSO.CardEffect.AttackSpeedIncrease:
                attackSpeed += attackSpeed * (card.effectValue / 100f);
                break;
        }
        OnStatsChanged?.Invoke(); // Lanza el evento una vez al final
    }

    // --- M�TODOS PRIVADOS ---
    private void Die()
    {
        if (playerSoundController != null)
        {
            playerSoundController.playsonidoMuerte(); 
        }
        Debug.Log("El jugador ha muerto.");
        gameObject.SetActive(false);
    }
}