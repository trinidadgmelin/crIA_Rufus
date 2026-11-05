using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class controlCinematica : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer; // Arrastra aquí tu Video Player
    public GameObject canvasConBoton; // Arrastra aquí el objeto de tu UI (Canvas o el panel con el botón)

    // Nombre o índice de la escena a la que quieres cambiar al terminar
    public string nombreEscenaDestino = "Escena2";

    void Start()
    {
        // Nos suscribimos al evento que se dispara cuando el video termina
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += AlTerminarVideo;
        }
    }

    // Método que llamarás desde el botón de la UI
    public void ReproducirCinematica()
    {
        // Ocultamos el canvas/botón para que no estorben durante el video
        if (canvasConBoton != null)
        {
            canvasConBoton.SetActive(false);
        }

        // Activamos y reproducimos el video
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
        }
    }

    // Este método se ejecuta automáticamente cuando el video llega al final
    void AlTerminarVideo(VideoPlayer vp)
    {
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}