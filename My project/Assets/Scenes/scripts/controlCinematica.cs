using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class controlCinematique : MonoBehaviour
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

    void Update()
    {
        // Verificamos únicamente si se presiona Enter en el teclado
        bool accionEnter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

        if (accionEnter)
        {
            // 1. Si el canvas con el botón de "Reproducir" sigue activo, lo accionamos
            if (canvasConBoton != null && canvasConBoton.activeInHierarchy)
            {
                ReproducirCinematica();
            }
            // 2. Si el video se está reproduciendo, al presionar Enter se salta y cambia de escena
            else if (videoPlayer != null && videoPlayer.isPlaying)
            {
                AlTerminarVideo(videoPlayer);
            }
        }
    }

    // Método que llamarás desde el botón de la UI o se disparará por teclado
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

    // Este método se ejecuta cuando el video termina o cuando se presiona Enter para saltarlo
    void AlTerminarVideo(VideoPlayer vp)
    {
        // Desuscribimos el evento para prevenir llamadas duplicadas
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= AlTerminarVideo;
        }

        SceneManager.LoadScene(nombreEscenaDestino);
    }
}