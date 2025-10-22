using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image barImg;

    // M�TODO CORREGIDO
    public void UpdateHealthBar(float vidaMaxima, float vidaActual)
    {
        // El c�lculo correcto es: vida actual / vida m�xima
        barImg.fillAmount = vidaActual / vidaMaxima;
    }
}