using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OutroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainMenu"; // O "Credits"

    void Start()
    {
        videoPlayer.frame = 0;
    videoPlayer.time = 0;
    videoPlayer.Stop(); // Por si acaso, detén cualquier reproducción previa.
    videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

}
