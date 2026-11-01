using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO.Ports;

public class nidoInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject objetoRufus;      // El GameObject de Rufus que se oculta al dormir

    [Header("Configuración del Fondo")]
    public SpriteRenderer fondoImage;
    public Sprite nuevoFondoSprite;     // El fondo que ya tiene a Rufus durmiendo dibujado
    private Sprite fondoOriginalSprite;

    [Header("Advertencia")]
    public GameObject sueñoAd;          // El cartel de advertencia de dormir

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    [Header("Configuración Arduino")]
    public string puertoSerial = "COM5";
    public int baudRate = 9600;
    private SerialPort stream;

    void Start()
    {
        if (fondoImage != null) fondoOriginalSprite = fondoImage.sprite;

        try
        {
            stream = new SerialPort(puertoSerial, baudRate);
            stream.ReadTimeout = 50;
            stream.Open();
        }
        catch (System.Exception e) { }
    }

    void Update()
    {
        bool activarPorTeclado = Input.GetKeyDown(KeyCode.B);
        bool activarPorSensor = false;

        if (stream != null && stream.IsOpen)
        {
            try
            {
                if (stream.BytesToRead > 0)
                {
                    string dato = stream.ReadLine().Trim();
                    if (dato.Contains("PRESIONADO") && dato.Contains("Pin 3"))
                    {
                        activarPorSensor = true;
                    }
                }
            }
            catch (System.Exception) { }
        }

        if (activarPorTeclado || activarPorSensor)
        {
            EjecutarAccion();
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen) stream.Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EjecutarAccion();
    }

    void EjecutarAccion()
    {
        // 1. Ocultar el objeto de Rufus para que solo se vea el del fondo nuevo
        if (objetoRufus != null)
        {
            objetoRufus.SetActive(false);
        }

        // 2. Apagar el cartel de advertencia al interactuar
        if (sueñoAd != null)
        {
            sueñoAd.SetActive(false);
        }

        // 3. Cambiar al fondo nuevo (que tiene a Rufus durmiendo)
        if (fondoImage != null && nuevoFondoSprite != null)
        {
            fondoImage.sprite = nuevoFondoSprite;
        }

        // 4. Reiniciar timer de Dormir
        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.quiereDormir = false;
            timerNecesidades.Instance.dormir.timerActual = timerNecesidades.Instance.dormir.intervalo;
            timerNecesidades.Instance.dormir.estaCorriendo = false;
        }

        StartCoroutine(RestaurarEstado());
    }

    IEnumerator RestaurarEstado()
    {
        yield return new WaitForSeconds(segundosVisible);

        // 5. Volver a mostrar el objeto de Rufus y restaurar el fondo original
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