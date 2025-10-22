using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Tooltip("Arrastra aquí el objeto StatsPanel desde la jerarquía")]
    [SerializeField] private GameObject statsPanel;

    // --- NUEVAS LÍNEAS ---
    [Tooltip("Arrastra aquí el objeto PauseMenuPanel desde la jerarquía")]
    [SerializeField] private GameObject pauseMenuPanel;
    private bool isPaused = false;
    // --------------------

    void Update()
    {
        // Toggle para el panel de estadísticas
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (statsPanel != null)
            {
                statsPanel.SetActive(!statsPanel.activeSelf);
            }
        }

        // --- NUEVA LÓGICA PARA LA PAUSA ---
        // Toggle para el menú de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- NUEVO MÉTODO PÚBLICO ---
    public void TogglePause()
    {
        isPaused = !isPaused; // Invierte el estado de pausa

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }

        // Pausa o reanuda el tiempo del juego
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // --- NUEVO MÉTODO PÚBLICO ---
    public void QuitGame()
    {
        Debug.Log("SALIENDO DEL JUEGO..."); // Mensaje para el editor
        Application.Quit(); // Cierra la aplicación (solo funciona en el juego compilado)
    }
}