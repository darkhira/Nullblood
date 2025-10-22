using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;

    private void OnEnable()
    {
        // Se suscribe para recibir futuras actualizaciones.
        PlayerStats.OnStatsChanged += UpdateUI;

        // --- ¡CAMBIO CLAVE! ---
        // Forzamos una actualización de la UI con los datos actuales
        // justo en el momento en que este panel se vuelve visible.
        UpdateUI();
    }

    private void OnDisable()
    {
        // Se desuscribe para evitar errores.
        if (PlayerStats.Instance != null)
        {
            PlayerStats.OnStatsChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        Debug.LogWarning("[StatsUI] Evento OnStatsChanged recibido. Actualizando la interfaz...");
        if (PlayerStats.Instance == null) return;

        // Obtenemos los datos más recientes de PlayerStats y actualizamos los textos.
        PlayerStats stats = PlayerStats.Instance;
        Debug.LogWarning($"[StatsUI] Nuevo valor de daño a mostrar: {stats.baseDamage}");

        if (damageText != null)
            damageText.text = $"Daño: {stats.baseDamage:F0}"; // "F1" formatea a 1 decimal

        if (healthText != null)
            healthText.text = $"Vida: {stats.currentHealth:F0}/{stats.maxHealth:F0}"; // "F0" para no tener decimales

        if (attackSpeedText != null)
            attackSpeedText.text = $"Vel. Ataque: {stats.attackSpeed:F1}";
    }
}