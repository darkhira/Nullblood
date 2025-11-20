using UnityEngine;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{
    public Slider brightnessSlider;
    public Image brightnessOverlay; // a fullscreen UI Image (black with transparency)

    void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat("brightness", 0.5f);
        brightnessSlider.value = savedBrightness;

        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    void OnBrightnessChanged(float value)
    {
        // Save for other scenes to read
        PlayerPrefs.SetFloat("brightness", value);
        PlayerPrefs.Save();
    }
}
