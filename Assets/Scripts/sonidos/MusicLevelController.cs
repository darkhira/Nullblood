using UnityEngine;

public class MusicLevelController : MonoBehaviour
{
    public AudioClip musicaFondo;

    void Start()
    {
        var audio = GetComponent<AudioSource>();
        audio.clip = musicaFondo;
        audio.loop = true;
        audio.Play();
    }
}
