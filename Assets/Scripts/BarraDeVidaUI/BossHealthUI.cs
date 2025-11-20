using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si quieres ponerle nombre al jefe con TextMeshPro

public class BossHealthUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject uiContainer; // El objeto padre para ocultar/mostrar todo
    [SerializeField] private TextMeshProUGUI bossNameText; // Opcional

    private void Awake()
    {
        // Al iniciar el juego, ocultamos la barra por si acaso el jefe no ha aparecido
        if (uiContainer != null) uiContainer.SetActive(false);
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
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public void DeactivateBossHealthBar()
    {
        if (uiContainer != null) uiContainer.SetActive(false);
    }
}