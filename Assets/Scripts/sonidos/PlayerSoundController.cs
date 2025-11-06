using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoPasillos;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoDash;
    public AudioClip sonidoPeleaKopp;
    public AudioClip sonidomuerteVorr;
    public AudioClip sonidoMuerteMono;
    public AudioClip sonidoEspacio;
    public AudioClip sonidoPistolaLaser;
    public AudioClip sonidoPuerta;
    public AudioClip sonidoClipMenu;
    public AudioClip sonidoRecibirDanio;
    public AudioClip sonidoPasillosdescartado;
    public AudioClip sonidoCofreAbierto;
    public AudioClip sonidoBoomerang;


    void Awake()
    {
        
        audioSource = GetComponent<AudioSource>();
    }

    public void playsonidoPasillos()
    {
        audioSource.PlayOneShot(sonidoPasillos);
    }

    public void playsonidoMuerte()
    {
        audioSource.PlayOneShot(sonidoMuerte);
    }

    public void playsonidoGolpe()
    {
        audioSource.PlayOneShot(sonidoGolpe);
    }

    public void playsonidoDash()
    {
        audioSource.PlayOneShot(sonidoDash);
    }

    public void playsonidoPeleaKopp()
    {
        audioSource.PlayOneShot(sonidoPeleaKopp);
    }

    public void playsonidomuerteKopp()
    {
        audioSource.PlayOneShot(sonidomuerteVorr);
    }

    public void playsonidoMuerteMono()
    {
        audioSource.PlayOneShot(sonidoMuerteMono);
    }

    public void playsonidoEspacio()
    {
        audioSource.PlayOneShot(sonidoEspacio);
    }

    public void playsonidoPistolaLaser()
    {
        audioSource.PlayOneShot(sonidoPistolaLaser);
    }

    public void playsonidoPuerta()
    {
        audioSource.PlayOneShot(sonidoPuerta);
    }

    public void playsonidoClipMenu()
    {
        audioSource.PlayOneShot(sonidoClipMenu);
    }

    public void playsonidoRecibirDanio()
    {
        audioSource.PlayOneShot(sonidoRecibirDanio);
    }

    public void playsonidoPasillosdescartado()
    {
        audioSource.PlayOneShot(sonidoPasillosdescartado);
    }

    public void playsonidoCofreAbierto()
    {
        audioSource.PlayOneShot(sonidoCofreAbierto);
    }

    public void playsonidoBoomerang()
    {
        audioSource.PlayOneShot(sonidoBoomerang);
    }

    
}
