using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Referencia al AudioMixer")]
    public AudioMixer audioMixer;

    [Header("Slider del volumen")]
    public Slider volumeSlider;

    private const string VolumeKey = "GameVolume";

    void Start()
    {
        // Cargar valor guardado
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.75f); // valor por defecto
        volumeSlider.value = savedVolume;

        SetVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        // Slider va de 0 a 1, AudioMixer usa dB → convertir logarítmicamente
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }
}
