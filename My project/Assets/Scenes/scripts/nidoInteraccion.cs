using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class nidoInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject objetoRufus;

    [Header("Configuración del Fondo")]
    public SpriteRenderer fondoImage;
    public Sprite nuevoFondoSprite;
    private Sprite fondoOriginalSprite;

    [Header("Advertencia")]
    public GameObject sueñoAd;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    void Start()
    {
        if (fondoImage != null) fondoOriginalSprite = fondoImage.sprite;
    }

    void Update()
    {
        if (ArduinoManager.Instance == null) return;

        bool activarPorTeclado = ArduinoManager.Instance.teclaBPresionada;
        bool activarPorSensor = ArduinoManager.Instance.sensorNidoPresionado;

        if (activarPorTeclado || activarPorSensor)
        {
            EjecutarAccion();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EjecutarAccion();
    }

    void EjecutarAccion()
    {
        if (objetoRufus != null)
        {
            objetoRufus.SetActive(false);
        }

        if (sueñoAd != null)
        {
            sueñoAd.SetActive(false);
        }

        if (fondoImage != null && nuevoFondoSprite != null)
        {
            fondoImage.sprite = nuevoFondoSprite;
        }

        // Reiniciar timer de Dormir y correr inmediatamente
        if (timerNecesidades.Instance != null)
            if (timerNecesidades.Instance != null)
            {
                timerNecesidades.Instance.quiereDormir = false;
                timerNecesidades.Instance.colaNecesidades.Remove("Dormir"); // <--- Quitamos de la cola
                timerNecesidades.Instance.dormir.timerActual = timerNecesidades.Instance.dormir.intervalo;
                timerNecesidades.Instance.dormir.estaCorriendo = true;
            }

        StartCoroutine(RestaurarEstado());
    }

    IEnumerator RestaurarEstado()
    {
        yield return new WaitForSeconds(segundosVisible);

        if (objetoRufus != null)
        {
            objetoRufus.SetActive(true);
        }

        if (fondoImage != null && fondoOriginalSprite != null)
        {
            fondoImage.sprite = fondoOriginalSprite;
        }
    }
}