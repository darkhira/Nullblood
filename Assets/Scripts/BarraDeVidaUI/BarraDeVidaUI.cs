using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarraDeVidaUI : MonoBehaviour
{
    [SerializeField] private Slider sliderBarraDeVida;
    // Opcional: Si quieres mostrar la vida en texto (ej. "80/100")
    [SerializeField] private TextMeshProUGUI healthText;

    private void OnEnable()
    {
        // --- CAMBIO: Nos suscribimos al único evento correcto: OnStatsChanged ---
        PlayerStats.OnStatsChanged += ActualizarUI;
    }

    private void OnDisable()
    {
        // --- CAMBIO: Nos desuscribimos del mismo evento ---
        if (PlayerStats.Instance != null)
        {
            PlayerStats.OnStatsChanged -= ActualizarUI;
        }
    }

    // Se llama una vez al inicio para configurar la barra
    private void Start()
    {
        // Forzamos una actualización inicial
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (PlayerStats.Instance != null)
        {
            // Actualizamos el slider
            sliderBarraDeVida.maxValue = PlayerStats.Instance.maxHealth;
            sliderBarraDeVida.value = PlayerStats.Instance.currentHealth;

            // Actualizamos el texto (si lo tienes asignado)
            if (healthText != null)
            {
                healthText.text = $"{PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth}";
            }
        }
    }
}