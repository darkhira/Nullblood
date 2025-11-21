using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance;

    [Header("Referencias UI")]
    [Tooltip("IMPORTANTE: Arrastra aquí el objeto HIJO que contiene los gráficos, NO este mismo objeto.")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI bossNameText;

    private void Awake()
    {
        // Configuración Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // --- BLOQUE DE SEGURIDAD ---
        if (uiContainer == gameObject)
        {
            Debug.LogError("Has asignado el mismo objeto 'BossHealthPanel' en la casilla 'Ui Container'.\n" +
                           "SOLUCIÓN: Debes crear un objeto hijo, meter los gráficos ahí y asignar ESE hijo a la casilla.");
            return; // Evitamos que se apague a sí mismo
        }
        // ---------------------------

        // Si todo está bien, ocultamos solo los gráficos
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }

    public void ActivateBossHealthBar(string bossName, float maxHealth)
    {
        if (uiContainer != null) uiContainer.SetActive(true);

        if (bossNameText != null) bossNameText.text = bossName;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void UpdateHealth(float currentHealth)
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    public void DeactivateBossHealthBar()
    {
        if (uiContainer != null) uiContainer.SetActive(false);
    }
}