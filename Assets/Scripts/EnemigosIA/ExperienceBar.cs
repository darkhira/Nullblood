using UnityEngine;
using UnityEngine.UI; // <--- NECESARIO para trabajar con Sliders, Images, etc.

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Slider experienceSlider;

    // Al activarse el objeto, nos suscribimos al evento del ExperienceManager.
    private void OnEnable()
    {
        ExperienceManager.OnExperienceChanged += UpdateExperienceBar;
    }

    // Al desactivarse, nos desuscribimos para evitar errores.
    private void OnDisable()
    {
        ExperienceManager.OnExperienceChanged -= UpdateExperienceBar;
    }

    /// <summary>
    /// Este método es llamado automáticamente por el evento OnExperienceChanged.
    /// </summary>
    /// <param name="currentExperience">La experiencia actual recibida del evento.</param>
    /// <param name="targetExperience">La experiencia objetivo recibida del evento.</param>
    private void UpdateExperienceBar(int currentExperience, int targetExperience)
    {
        if (experienceSlider != null)
        {
            // El valor de un slider va de 0 a 1.
            // Dividimos la experiencia actual entre el total para obtener el progreso.
            experienceSlider.value = (float)currentExperience / targetExperience;
        }
    }
}