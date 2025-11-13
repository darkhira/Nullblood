using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoPasillos;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoDash;
    public AudioClip sonidoPeleaKopp;
    public AudioClip sonidoMuerteVorr;
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
        
        audioSource.clip = sonidoPasillos; 
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    
        audioSource.Play();

    }

    public void playsonidoMuerte()
{
    audioSource.volume = 1.0f;
    
    if (sonidoMuerte != null)
    {
        
        AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position);
    }
}

    public void playsonidoGolpe()
    {
        audioSource.PlayOneShot(sonidoGolpe);
        audioSource.volume = 0.5f;
    }

    public void playsonidoDash()
    {
        audioSource.PlayOneShot(sonidoDash);
        audioSource.volume = 0.5f;
    }

    public void playsonidoPeleaKopp()
    {
        audioSource.PlayOneShot(sonidoPeleaKopp);
    }

    public void playsonidoMuerteVorr()
    {
        if (sonidoMuerteVorr != null)
        {
        
        AudioSource.PlayClipAtPoint(sonidoMuerteVorr, transform.position);
        }
    }

    public void playsonidoMuerteMono()
    {
        if (sonidoMuerteMono != null)
        {
        
        AudioSource.PlayClipAtPoint(sonidoMuerteMono, transform.position);
        }
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
        audioSource.volume = 0.3f;
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
        audioSource.volume = 0.2f;
    }

    
}
