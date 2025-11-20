using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausemenu : MonoBehaviour
{
    private bool _isGamePaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.JoystickButton19))
        {
            if (!_isGamePaused)
            {
                _isGamePaused = true;
                SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
                Time.timeScale = 0.0f;
            }
            else if (_isGamePaused)
            {
                _isGamePaused = false;
                SceneManager.UnloadSceneAsync("OptionsMenu");
                Time.timeScale = 1.0f;
            }
        }
    }
}
