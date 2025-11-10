using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausemenu : MonoBehaviour
{
    private bool _isGamePaused = false;
    [Tooltip("Arrastra el objeto PauseMenuPanel (el Canvas o panel que muestra el menú de pausa)")]
    [SerializeField] private GameObject pauseMenuPanel;
    [Tooltip("Referencia al UIManager de la escena para centralizar la pausa")]
    [SerializeField] private UIManager uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If uiManager not assigned, try to find one in the scene
        if (uiManager == null)
        {
            uiManager = UnityEngine.Object.FindFirstObjectByType<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.JoystickButton19))
        {
            if (!_isGamePaused)
            {
                _isGamePaused = true;
                // Ocultamos el panel de pausa de la escena base para que no sea interactuable
                if (pauseMenuPanel != null)
                {
                    // Desactivar para que no reciba eventos ni sea visible
                    pauseMenuPanel.SetActive(false);
                }
                // Notify UIManager and stop time centrally
                if (uiManager != null)
                {
                    uiManager.SetPause(true);
                }
                SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
            }
            else if (_isGamePaused)
            {
                _isGamePaused = false;
                SceneManager.UnloadSceneAsync("OptionsMenu");
                // Volvemos a activar el panel de pausa cuando cerramos opciones
                if (pauseMenuPanel != null)
                {
                    pauseMenuPanel.SetActive(true);
                }
                if (uiManager != null)
                {
                    uiManager.SetPause(false);
                }
            }
        }
    }
}
