using UnityEngine;
using UnityEngine.Video; // Necesario para controlar videos
using UnityEngine.SceneManagement;

public class CinematicVideoManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nombreEscenaJuego = "SampleScene"; // Pon el nombre EXACTO de tu escena de juego
    [SerializeField] private KeyCode teclaSaltar = KeyCode.Space;

    void Start()
    {
        // Si se nos olvidó arrastrarlo, intentamos buscarlo en este objeto
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Nos suscribimos al evento: "Cuando el video llegue al final..."
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        // Permitir saltar la intro
        if (Input.GetKeyDown(teclaSaltar) || Input.GetKeyDown(KeyCode.Escape))
        {
            CargarJuego();
        }
    }

    // Este método se llama automáticamente cuando el video termina
    void OnVideoFinished(VideoPlayer vp)
    {
        CargarJuego();
    }

    void CargarJuego()
    {
        // Buena práctica: Desuscribirse del evento para limpiar memoria
        videoPlayer.loopPointReached -= OnVideoFinished;

        Debug.Log("Video terminado o saltado. Cargando juego...");
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}