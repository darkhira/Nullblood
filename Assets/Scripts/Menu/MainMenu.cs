using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    private void Start()
    {
        // If there's a UIManager in the scene or a DontDestroyOnLoad one, mark it
        // as MainMenu mode so it ignores Escape toggles that belong to gameplay.
        UIManager ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.SetModeMainMenu(true);
        }
    }

    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        // Switch UIManager back to Gameplay mode if present
        UIManager ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.SetModeMainMenu(false);
        }
        SceneManager.LoadScene("SampleScene");
    }

}
