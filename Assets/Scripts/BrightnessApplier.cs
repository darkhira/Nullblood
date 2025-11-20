using UnityEngine;
using UnityEngine.UI;

public class BrightnessApplier : MonoBehaviour
{
    [SerializeField]private Image brightnessOverlay; // assign in Inspector

    void Start()
    {
        ApplySavedBrightness();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ApplySavedBrightness();
        }
    }

    void ApplySavedBrightness()
    {
        float brightness = PlayerPrefs.GetFloat("brightness", 0.5f);

        // Adjust alpha: higher brightness = lower darkness overlay
        Color color = brightnessOverlay.color;
        color.a = 1f - brightness;
        brightnessOverlay.color = color;
    }
}
