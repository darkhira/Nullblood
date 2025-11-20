using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Optionsmenu : MonoBehaviour
{
    public String sceneName1;
    [SerializeField] private UIManager UIManager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnloadScene();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadSceneByName(string sceneName)
    {
        // Asegúrate de que la escena esté en "Build Settings"
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        UIManager.TogglePause();
    }
    public void UnloadScene()
    {
        // Check if the scene is actually loaded before trying to unload
        Scene scene = SceneManager.GetSceneByName(sceneName1);
        if (scene.isLoaded)
        {
            UIManager.TogglePause();
            SceneManager.UnloadSceneAsync(sceneName1);
            Debug.Log($"Scene '{sceneName1}' unloaded successfully!");
            
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName1}' is not loaded.");
        }
    }
}
