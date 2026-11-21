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
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += AlTerminarVideo;
        }
    }

    void Update()
    {
        bool accionEnter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        bool accionBotonSel = ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado;

        if (accionEnter || accionBotonSel)
        {
            // 1. Si el canvas con el botón de "Reproducir" sigue activo, lo accionamos
            if (canvasConBoton != null && canvasConBoton.activeInHierarchy)
            {
                ReproducirCinematica();
            }
            // 2. Si el video se está reproduciendo, al presionar Enter o BOTON_SEL se salta y cambia de escena
            else if (videoPlayer != null && videoPlayer.isPlaying)
            {
                AlTerminarVideo(videoPlayer);
            }
        }
    }

    public void ReproducirCinematica()
    {
        if (canvasConBoton != null)
        {
            canvasConBoton.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
        }
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= AlTerminarVideo;
        }

        SceneManager.LoadScene(nombreEscenaDestino);
    }
}