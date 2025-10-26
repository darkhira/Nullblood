using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    // Se llama cuando el script se carga.
    private void Awake()
    {
        // Si no has arrastrado el Slider en el Inspector, intenta encontrarlo en este mismo objeto.
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }
    }

    // Se llama cuando el objeto se activa.
    private void OnEnable()
    {
        // Se suscribe al evento para recibir actualizaciones de vida.
        PlayerStats.OnStatsChanged += UpdateHealthBar;
        UpdateHealthBar(); // Llama una vez para tener el valor inicial correcto.
    }

    // Se llama cuando el objeto se desactiva.
    private void OnDisable()
    {
        // Se desuscribe del evento para evitar errores.
        if (PlayerStats.Instance != null)
        {
            PlayerStats.OnStatsChanged -= UpdateHealthBar;
        }
    }

    // Método que actualiza la barra de vida visualmente.
    private void UpdateHealthBar()
    {
        if (PlayerStats.Instance != null && healthSlider != null)
        {
            healthSlider.maxValue = PlayerStats.Instance.maxHealth;
            healthSlider.value = PlayerStats.Instance.currentHealth;
        }
    }
}