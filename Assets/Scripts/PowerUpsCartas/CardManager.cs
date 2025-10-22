using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    [SerializeField] GameObject cardSelectionUI;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] private List<CardSO> deck;

    [Header("Posiciones Dinámicas")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("Distancia entre las cartas.")]
    [SerializeField] private float cardSpacing = 4f;
    [Tooltip("Qué tan por encima del jugador aparecerán las cartas.")]
    [SerializeField] private float verticalOffset = 1f;

    GameObject cardOne, cardTwo, cardThree;
    List<CardSO> alreadySelectedCards = new List<CardSO>();
    public static CardManager Instance;

    private void Awake()
    {
        Instance = this;
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.CardSelection)
        {
            RandomizeNewCards();
        }
    }

    void RandomizeNewCards()
    {
        List<CardSO> randomizedCards = new List<CardSO>();
        List<CardSO> availableCards = new List<CardSO>(deck);
        availableCards.RemoveAll(card =>
            card.isUnique && alreadySelectedCards.Contains(card)
         || card.unlockLevel > GameManager.Instance.GetCurrentLevel()
            );

        if (availableCards.Count < 3)
        {
            Debug.Log("No hay suficientes cartas disponibles para seleccionar.");
            return;
        }

        while (randomizedCards.Count < 3)
        {
            CardSO randomCard = availableCards[Random.Range(0, availableCards.Count)];
            if (!randomizedCards.Contains(randomCard))
            {
                randomizedCards.Add(randomCard);
            }
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform no está asignado en el CardManager!");
            return;
        }

        // --- CAMBIO: Añadimos ".x" y ".y" y definimos Z manualmente ---
        float newZ = -1f; // Las cartas estarán más cerca de la cámara que el jugador (que está en Z=0)
        Vector3 centerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + verticalOffset, newZ);
        Vector3 leftPos = new Vector3(centerPos.x - cardSpacing, centerPos.y, newZ);
        Vector3 rightPos = new Vector3(centerPos.x + cardSpacing, centerPos.y, newZ);

        cardOne = InstantiateCard(randomizedCards[0], leftPos);
        cardTwo = InstantiateCard(randomizedCards[1], centerPos);
        cardThree = InstantiateCard(randomizedCards[2], rightPos);
    }

    GameObject InstantiateCard(CardSO cardSO, Vector3 position)
    {
        GameObject cardGo = Instantiate(cardPrefab, position, Quaternion.identity, transform);
        Card card = cardGo.GetComponent<Card>();
        card.Setup(cardSO);
        return cardGo;
    }

    public void SelectCard(CardSO selectedCard)
    {
        if (!alreadySelectedCards.Contains(selectedCard))
        {
            alreadySelectedCards.Add(selectedCard);
        }

        // --- LÍNEA DE PRUEBA: FUERZA LA REACTIVACIÓN DEL JUGADOR ---
        if (GameObject.FindGameObjectWithTag("Player").activeInHierarchy == false)
        {
            Debug.LogError("¡DIAGNÓSTICO: El Player ESTABA INACTIVO! Reactivándolo a la fuerza.");
            GameObject.FindGameObjectWithTag("Player").SetActive(true);
        }
        // ----------------------------------------------------------------

        Debug.LogWarning($"[CardManager] Carta '{selectedCard.name}' seleccionada. Llamando a PlayerStats para aplicar efecto...");
        PlayerStats.Instance.ApplyCardEffect(selectedCard);

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);

        ClearInstantiatedCards();
    }

    private void ClearInstantiatedCards()
    {
        if (cardOne != null) Destroy(cardOne);
        if (cardTwo != null) Destroy(cardTwo);
        if (cardThree != null) Destroy(cardThree);
    }

    public void ShowCardSelection()
    {
        cardSelectionUI.SetActive(true);
    }

    public void HideCardSelection()
    {
        cardSelectionUI.SetActive(false);
    }
}