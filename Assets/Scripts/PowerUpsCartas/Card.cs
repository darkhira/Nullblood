using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer cardImageRenderer;
    [SerializeField] TextMeshPro cardTextRenderer;
    private CardSO cardInfo;
    public void Setup(CardSO card)
    {
        cardInfo = card;
        cardImageRenderer.sprite = card.cardImage;
        cardTextRenderer.text = card.cardText;
    }

    void OnMouseDown()
    {
        Debug.Log("Tu clickeaste la carta");
        CardManager.Instance.SelectCard(cardInfo);
    }
}

