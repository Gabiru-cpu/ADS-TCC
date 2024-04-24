using UnityEngine;
using UnityEngine.Video;

public class ChangeOnFinish : MonoBehaviour
{
    public LoadingScreenBarSystem loadingScreen;

    void Start()
    {
        // Chama VideoEnd ap�s 35 segundos
        Invoke("VideoEnd", 34f);
    }

    void VideoEnd()
    {
        loadingScreen.loadingScreen(2);
    }
}