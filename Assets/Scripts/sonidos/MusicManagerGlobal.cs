using UnityEngine;
using UnityEngine.SceneManagement; // ¡Muy importante para detectar cambios de escena!

[RequireComponent(typeof(AudioSource))]
public class MusicManagerGlobal : MonoBehaviour
{
    // --- Campos para asignar en el Inspector ---
    [Header("Música del Menú")]
    public AudioClip musicaDelMenu;
    public string nombreEscenaMenu = "MainMenu"; // <-- ¡Escribe el nombre de tu escena de Menú!

    [Header("Música del Juego")]
    public AudioClip musicaDelJuego;
    public string nombreEscenaJuego = "Nivel1"; // <-- ¡Escribe el nombre de tu escena de Juego!
    // ------------------------------------------


    public static MusicManagerGlobal instance;
    private AudioSource audioSource;

    void Awake()
    {
        // 1. El Patrón Singleton (para que solo exista UNO)
        if (instance == null)
        {
            // Si soy el primero, me quedo
            instance = this;
            DontDestroyOnLoad(gameObject); // ¡No me destruyas al cargar escena!
        }
        else
        {
            // Si ya existe uno (ej. volví al menú), este nuevo se destruye.
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // La música siempre debe ser en loop
    }

    // 2. Nos suscribimos a los eventos de Unity
    void OnEnable()
    {
        // Le decimos a Unity: "Oye, avísame cuando una escena se haya cargado"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // "Ya no me avises" (buena práctica)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 3. Este método se llama CADA VEZ que una escena termina de cargar
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string nombreEscenaActual = scene.name;

        // Comparamos el nombre de la escena con los que definimos
        if (nombreEscenaActual == nombreEscenaMenu)
        {
            CambiarMusica(musicaDelMenu);
        }
        else if (nombreEscenaActual == nombreEscenaJuego)
        {
            CambiarMusica(musicaDelJuego);
        }
        // Puedes añadir más 'else if' para un "Nivel2" con otra música
    }

    // 4. Un método seguro para cambiar la música
    private void CambiarMusica(AudioClip nuevaMusica)
    {
        // Si la nueva música es nula O es la misma que ya está sonando, no hacemos nada.
        if (nuevaMusica == null || audioSource.clip == nuevaMusica)
        {
            return;
        }

        // Si es una música diferente, la cambiamos.
        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.Play();
    }
}