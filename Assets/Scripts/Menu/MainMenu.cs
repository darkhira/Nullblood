using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuración de UI")]
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Configuración de Audio")]
    [Tooltip("Arrastra aquí el objeto que tiene el AudioSource con la música del menú")]
    public AudioSource musicaDeFondo; // <--- NUEVA VARIABLE

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
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void PlayGame()
    {
        // 1. Silenciar la música antes de irnos
        if (musicaDeFondo != null)
        {
            musicaDeFondo.Stop(); // Detiene la música de golpe
        }

        // 2. Cargar la cinemática (o el juego)
        SceneManager.LoadScene("IntroCinematic");
    }
}