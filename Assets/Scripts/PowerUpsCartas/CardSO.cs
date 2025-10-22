using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardImage; //La imagen de la carta
     
    public string cardText; //El texto que describe la carta

    public CardEffect effectType; // El tipo de efecto que tiene la carta

    public float effectValue; // El valor del efecto que tiene la carta (10%)

    public bool isUnique; // Si la carta es unica o no

    public int unlockLevel; // El nivel en el que se desbloquea la carta


    public enum CardEffect
    {
        DamageIncrease,
        HealthIncrease,
        AttackSpeedIncrease,
    }
}
