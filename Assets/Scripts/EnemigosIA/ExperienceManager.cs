using System; //
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    // --- NUEVO EVENTO ---
    // Este evento notificar� a los suscriptores (como la UI) cuando la XP cambie.
    // Env�a la experiencia actual y la necesaria para el siguiente nivel.
    public static event Action<int, int> OnExperienceChanged;

    [Tooltip("Enemigos que se deben derrotar para subir de nivel.")]
    [SerializeField] private int experienceToLevelUp = 10;

    private int currentExperience = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // --- NUEVA L�NEA ---
        // Al empezar, notifica a la UI los valores iniciales (0 de 10, por ejemplo).
        OnExperienceChanged?.Invoke(currentExperience, experienceToLevelUp);
    }

    public void AddExperience()
    {
        currentExperience++;
        Debug.Log($"[XP Manager] Enemigo derrotado. Progreso: {currentExperience}/{experienceToLevelUp}");

        // --- NUEVA L�NEA ---
        // Notificamos a todos los suscriptores del cambio en la experiencia.
        OnExperienceChanged?.Invoke(currentExperience, experienceToLevelUp);

        if (currentExperience >= experienceToLevelUp)
        {
            currentExperience = 0;
            Debug.Log("[XP Manager] �NIVEL ALCANZADO! Solicitando subida de nivel al GameManager.");
            GameManager.Instance.TriggerLevelUp();

            // --- NUEVA L�NEA ---
            // Notificamos de nuevo para que la barra se resetee a cero.
            OnExperienceChanged?.Invoke(currentExperience, experienceToLevelUp);
        }
    }
}