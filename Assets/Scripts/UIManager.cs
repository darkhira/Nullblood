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
    // When true, UIManager should ignore global input like Escape because
    // another overlay/menu (for example an additive Options scene) is handling it.
    public static bool InputBlocked = false;
    // --------------------
    public enum UIMode { Gameplay, MainMenu }
    [Tooltip("Modo en que se encuentra este UIManager: Gameplay permite toggle de pausa, MainMenu ignora Escape para evitar cerrar el menú principal accidentalmente")]
    [SerializeField] private UIMode mode = UIMode.Gameplay;

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
            // If input is globally blocked by another overlay (e.g. Options scene)
            // do not process Escape here.
            if (InputBlocked) return;

            // If this UIManager is running in MainMenu mode, ignore Escape here
            // to avoid closing/opening the main menu when overlays are active.
            if (mode == UIMode.MainMenu) return;

            // Only toggle pause if this UIManager actually controls a pause panel
            // that belongs to the active scene. This avoids toggling pause from
            // scenes like the main menu or other additive UIs.
            if (pauseMenuPanel != null && pauseMenuPanel.scene == SceneManager.GetActiveScene())
            {
                TogglePause();
            }
        }
    }

    public void SetMode(UIMode newMode)
    {
        mode = newMode;
    }

    public void SetModeMainMenu(bool isMainMenu)
    {
        mode = isMainMenu ? UIMode.MainMenu : UIMode.Gameplay;
    }
    
    // Query helper for other scripts
    public bool IsMainMenuMode()
    {
        return mode == UIMode.MainMenu;
    }

    // --- NUEVO M�TODO P�BLICO ---
    public void TogglePause()
    {
        SetPause(!isPaused);
    }

    // Centraliza la lógica de pausa para evitar inconsistencias entre scripts
    public void SetPause(bool paused)
    {
        isPaused = paused;

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

    // --- NUEVO M�TODO P�BLICO ---
    public void QuitGame()
    {
        Debug.Log("SALIENDO DEL JUEGO..."); // Mensaje para el editor
        Application.Quit(); // Cierra la aplicaci�n (solo funciona en el juego compilado)
    }
}