using UnityEngine;

// Esto se asegura de que el AudioSource exista
[RequireComponent(typeof(AudioSource))] 
public class MusicaDeFondoNivel1 : MonoBehaviour
{
    // 1. Arrastra tu archivo de música aquí en el Inspector
    public AudioClip musicaDelNivel; 

    private AudioSource audioSource;

    void Start()
    {
        // 2. Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // 3. Configura la música
        audioSource.clip = musicaDelNivel; // Asigna tu música
        audioSource.loop = true;          // Para que se repita
        audioSource.playOnAwake = true;   // Que suene al empezar
        audioSource.volume = 0.5f;        // Ajusta el volumen a tu gusto

        // 4. ¡Reproduce!
        audioSource.Play();
    }
}