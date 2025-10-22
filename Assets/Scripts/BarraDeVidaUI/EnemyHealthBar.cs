using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image barImg;

    // MÉTODO CORREGIDO
    public void UpdateHealthBar(float vidaMaxima, float vidaActual)
    {
        // El cálculo correcto es: vida actual / vida máxima
        barImg.fillAmount = vidaActual / vidaMaxima;
    }
}