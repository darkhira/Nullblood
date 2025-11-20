using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer audioMixer;
    private const string VolumeKey = "GameVolume";

    void Awake()
    {
        // Singleton (opcional)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Aplicar volumen guardado
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.75f);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(savedVolume) * 20);
    }
}
