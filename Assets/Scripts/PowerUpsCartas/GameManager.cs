using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool escudoActivo = false;
    int currentLevel = 0;
    GameState currentState;
    public event Action<GameState> OnStateChanged;

    // --- NUEVAS VARIABLES ---
    [Header("Conteo de Enemigos")]
    [Tooltip("Enemigos que se deben derrotar para que aparezcan las cartas.")]
    [SerializeField] private int enemigosParaCartas = 10;

    private int enemigosDerrotadosContador = 0;
    // -------------------------

    private void Awake()
    {
        // Nos aseguramos de que el tiempo corra al iniciar esta escena
        Time.timeScale = 1f;
        Instance = this;
    }
    public void ActivarEscudoGlobal()
{
    escudoActivo = true;
}
public void DesactivarEscudoGlobal()
{
    escudoActivo = false;
}

    private void Update()
    {
        // Mantenemos la tecla 'J' para pruebas.
        if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerCardSelection();
        }
    }

    // --- NUEVO M�TODO P�BLICO ---
    /// <summary>
    /// Llamado por el script 'Enemigo' cada vez que uno muere.
    /// </summary>
    public void OnEnemyKilled()
    {
        enemigosDerrotadosContador++;
        Debug.Log($"[GameManager] Enemigo derrotado. Progreso: {enemigosDerrotadosContador} / {enemigosParaCartas}");

        // Comprueba si se ha alcanzado el objetivo
        if (enemigosDerrotadosContador >= enemigosParaCartas)
        {
            // Resetea el contador y muestra las cartas
            enemigosDerrotadosContador = 0;
            TriggerCardSelection();
        }
    }

    // --- NOMBRE CAMBIADO: De 'TriggerLevelUp' a 'TriggerCardSelection' ---
    public void TriggerCardSelection()
    {
        // Seguimos aumentando el nivel para el desbloqueo de cartas, 
        // pero la funci�n ahora tiene un nombre m�s claro.
        currentLevel++;
        Debug.Log("�CONTEO ALCANZADO! Mostrando selecci�n de cartas.");
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