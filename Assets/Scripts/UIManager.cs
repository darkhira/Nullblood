using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Tooltip("Arrastra aqu� el objeto StatsPanel desde la jerarqu�a")]
    [SerializeField] private GameObject statsPanel;

    // --- NUEVAS L�NEAS ---
    [Tooltip("Arrastra aqu� el objeto PauseMenuPanel desde la jerarqu�a")]
    [SerializeField] private GameObject pauseMenuPanel;
    private bool isPaused = false;
    // --------------------

    void Update()
    {
        // Toggle para el panel de estad�sticas
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (statsPanel != null)
            {
                statsPanel.SetActive(!statsPanel.activeSelf);
            }
        }

        // --- NUEVA L�GICA PARA LA PAUSA ---
        // Toggle para el men� de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            StopTime();
        }
    }

    // --- NUEVO M�TODO P�BLICO ---
    public void TogglePause()
    {
        isPaused = !isPaused; // Invierte el estado de pausa

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }

        // Pausa o reanuda el tiempo del juego
        
    }
    public void StopTime()
    {
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // --- NUEVO M�TODO P�BLICO ---
    public void QuitGame()
    {
        Debug.Log("SALIENDO DEL JUEGO..."); // Mensaje para el editor
        Application.Quit(); // Cierra la aplicaci�n (solo funciona en el juego compilado)
    }
}