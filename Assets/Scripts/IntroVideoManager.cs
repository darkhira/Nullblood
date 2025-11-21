using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string menuSceneName = "MainMenu";
    private bool hasSkipped = false;

    void Start()
    {

    videoPlayer.frame = 0;
    videoPlayer.time = 0;
    videoPlayer.Stop(); // Por si acaso, detén cualquier reproducción previa.
    videoPlayer.Play();
    videoPlayer.loopPointReached += OnVideoFinished;
    Debug.Log("IntroVideoManager iniciado");


        
    }

    void Update()
    {
        // Salta el video con cualquier input (tecla o botón de mando)
        if (!hasSkipped && (Input.anyKeyDown || AnyJoystickButton()))
        {
            Debug.Log("Intro saltada por input");
            hasSkipped = true;
            GoToMenu();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado, cambiando de escena");
        GoToMenu();
    }

    void GoToMenu()
    {
        Debug.Log($"Intentando cargar escena: {menuSceneName}");
        SceneManager.LoadScene(menuSceneName);
    }

    // Detecta si cualquier botón del mando fue presionado
    bool AnyJoystickButton()
    {
        for (KeyCode k = KeyCode.JoystickButton0; k <= KeyCode.JoystickButton19; k++)
        {
            if (Input.GetKeyDown(k))
                return true;
        }
        return false;
    }
}
