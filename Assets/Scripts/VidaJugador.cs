using System;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    public PlayerSoundController playerSoundController;
    public Action<int> JugadorSeCuro;
    public Action<int> JugadorTomoDaño;


    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;

    void Start()
    {
        playerSoundController = GetComponent<PlayerSoundController>();
    }
    private void Awake()
    {


        vidaActual = vidaMaxima;
    }

    public void TomarDaño(int daño)
    {

        int vidaTemporal = vidaActual - daño;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        JugadorTomoDaño?.Invoke(vidaActual);

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
