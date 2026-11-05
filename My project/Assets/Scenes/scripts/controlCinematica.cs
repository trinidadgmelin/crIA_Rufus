using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class controlCinematique : MonoBehaviour // Mantén el nombre de tu clase original (controlCinematica)
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
        // Verificamos si se presiona Enter en el teclado O el botón central del Arduino
        bool accionSeleccionar = Input.GetKeyDown(KeyCode.Return) ||
                                 Input.GetKeyDown(KeyCode.KeypadEnter) ||
                                 (ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado);

        if (accionSeleccionar)
        {
            // Si el canvas con el botón de "Reproducir" sigue activo, lo accionamos
            if (canvasConBoton != null && canvasConBoton.activeInHierarchy)
            {
                ReproducirCinematica();
            }
            // Opcional: Si querés que el botón central también sirva para saltar/cortar el video antes de que termine, podés descomentar la línea de abajo:
            /*
            else if (videoPlayer != null && videoPlayer.isPlaying)
            {
                AlTerminarVideo(videoPlayer);
            }
            */
        }
    }

    // Método que llamarás desde el botón de la UI o se disparará por teclado/Arduino
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