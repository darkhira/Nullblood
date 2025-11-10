using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Optionsmenu : MonoBehaviour
{
    public String sceneName1;
    [SerializeField] private UIManager UIManager;
    // CanvasGroups we disabled when opening the options scene; used to restore on close
    private List<CanvasGroup> disabledCanvasGroups = new List<CanvasGroup>();
    // Selectable UI elements (Buttons, Toggles, Dropdowns) disabled when opening options
    private List<UnityEngine.UI.Selectable> disabledSelectables = new List<UnityEngine.UI.Selectable>();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Only try to unload if this options scene is actually loaded.
            Scene scene = SceneManager.GetSceneByName(sceneName1);
            if (scene.isLoaded)
            {
                UnloadScene();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadSceneByName(string sceneName)
    {
        // Asegúrate de que la escena esté en "Build Settings"
        // Before loading, disable interaction on canvases from other loaded scenes
        // so their buttons can't be selected while options overlay is open.
        disabledCanvasGroups.Clear();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var sc = SceneManager.GetSceneAt(i);
            if (!sc.isLoaded) continue;
            // skip the scene we are about to open (it isn't loaded yet)
            if (sc.name == sceneName) continue;
            var roots = sc.GetRootGameObjects();
            foreach (var root in roots)
            {
                var groups = root.GetComponentsInChildren<CanvasGroup>(true);
                foreach (var g in groups)
                {
                    // only disable if currently interactable/blocksRaycasts
                    if (g.interactable || g.blocksRaycasts)
                    {
                        g.interactable = false;
                        g.blocksRaycasts = false;
                        disabledCanvasGroups.Add(g);
                    }
                }
                // Also disable any Selectable UI elements (buttons, toggles, etc.)
                var selectables = root.GetComponentsInChildren<UnityEngine.UI.Selectable>(true);
                foreach (var s in selectables)
                {
                    if (s != null && s.interactable)
                    {
                        s.interactable = false;
                        disabledSelectables.Add(s);
                    }
                }
            }
        }

        // Clear current selection so navigation doesn't jump back to hidden buttons
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        // Block the main UI so it doesn't react to Escape while this additive
        // options scene is open. Use SetPause(true) to pause if a UIManager
        // is provided (safer than TogglePause which toggles state).
        if (UIManager != null)
        {
            UIManager.InputBlocked = true;
            UIManager.SetPause(true);
        }
    }
    public void UnloadScene()
    {
        // Check if the scene is actually loaded before trying to unload
        Scene scene = SceneManager.GetSceneByName(sceneName1);
        if (scene.isLoaded)
        {
            // Restore input handling and unpause via UIManager if available.
            // IMPORTANT: re-enable InputBlocked in the next frame to avoid the
            // Escape key being processed by other UIManagers in the same frame
            // (which would close the main menu). Use a coroutine to delay one frame.
            if (UIManager != null)
            {
                // Only unpause the UIManager if it's not in MainMenu mode. When
                // the UIManager is in MainMenu mode we don't want to toggle its
                // pause state (that can close the main menu).
                if (!UIManager.IsMainMenuMode())
                {
                    UIManager.SetPause(false);
                }
                StartCoroutine(RestoreInputNextFrame());
            }

            // Restore any CanvasGroups we disabled when opening options
            foreach (var g in disabledCanvasGroups)
            {
                if (g != null)
                {
                    g.interactable = true;
                    g.blocksRaycasts = true;
                }
            }
            // Restore any Selectables we disabled
            foreach (var s in disabledSelectables)
            {
                if (s != null)
                {
                    s.interactable = true;
                }
            }
            disabledSelectables.Clear();
            disabledCanvasGroups.Clear();
            SceneManager.UnloadSceneAsync(sceneName1);
            Debug.Log($"Scene '{sceneName1}' unloaded successfully!");
            
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName1}' is not loaded.");
        }
    }

    private System.Collections.IEnumerator RestoreInputNextFrame()
    {
        // keep input blocked this frame, then re-enable next frame to avoid
        // propagation of the Escape key to other UIManagers.
        yield return null;
        if (UIManager != null)
        {
            UIManager.InputBlocked = false;
        }
    }
}
