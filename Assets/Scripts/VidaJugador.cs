using System;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    public Action<int> JugadorSeCuro;
    public Action<int> JugadorTomoDa�o;


    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    public void TomarDa�o(int da�o)
    {

        int vidaTemporal = vidaActual - da�o;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        JugadorTomoDa�o?.Invoke(vidaActual);

        if (vidaActual <= 0)
        {
            DestruirJugador();
        }
    }

    public void CurarVida(int curacion)
    {

        int vidaTemporal = vidaActual + curacion;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        JugadorSeCuro?.Invoke(vidaActual);
    }

    private void DestruirJugador()
    {
        Destroy(gameObject);
    }

    public int GetVidaMaxima() => vidaMaxima;

    public int GetVidaActual() => vidaActual;
}
