using UnityEngine;
using UnityEngine.UI;

public class FullscreenOption : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;

    void Start()
    {
        // Load the saved fullscreen state
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;

        // Apply it immediately
        Screen.fullScreen = isFullscreen;

        // Listen for changes
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
