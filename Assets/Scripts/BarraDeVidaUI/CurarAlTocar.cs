using UnityEngine;

public class CurarAlTocar : MonoBehaviour
{
    [SerializeField] private int cantidadCuracion;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out VidaJugador vidaJugador))
        {
            vidaJugador.CurarVida(cantidadCuracion);
            Destroy(gameObject);
        }
    }
}
