using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    int currentLevel = 0;
    GameState currentState;
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        Time.timeScale = 1f;

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerLevelUp();
        }
    }

    public void TriggerLevelUp()
    {
        currentLevel++;
        Debug.Log("¡NIVEL AUMENTADO A: " + currentLevel + "! Mostrando selección de cartas.");
        ChangeState(GameState.CardSelection);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(newState);
        HandleStateChanged();
    }

    private void HandleStateChanged()
    {
        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                CardManager.Instance.HideCardSelection();
                break;

            case GameState.CardSelection:
                Time.timeScale = 0f;
                CardManager.Instance.ShowCardSelection();
                break;
        }
    }

    public enum GameState
    {
        Playing,
        CardSelection,
    }
}