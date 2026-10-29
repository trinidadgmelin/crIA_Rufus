using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO.Ports;

public class mateInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject mate;
    public Image rufusFeliz;
    public Sprite rufusMate;

    [Header("Configuración Arduino")]
    public string puertoSerial = "COM5"; // Cambiá esto si tu puerto es otro (ej. COM4)
    public int baudRate = 9600;
    private SerialPort stream;

    void Start()
    {
        try
        {
            stream = new SerialPort(puertoSerial, baudRate);
            stream.ReadTimeout = 50;
            stream.Open();
            Debug.Log("Puerto serie abierto correctamente en " + puertoSerial);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo abrir el puerto serie: " + e.Message);
        }
    }

    void Update()
    {
        // 1. Detección por teclado (B o M + Flecha Derecha)
        bool activarPorTeclado = Input.GetKeyDown(KeyCode.B);

        bool activarPorSensor = false;

        // 2. Detección por Puerto Serie (Arduino)
        if (stream != null && stream.IsOpen)
        {
            try
            {
                // Solo leemos si hay bytes esperando en el búfer
                if (stream.BytesToRead > 0)
                {
                    string dato = stream.ReadLine().Trim();

                    // ¡Esto te mostrará en la consola lo que manda Arduino!
                    Debug.Log("Arduino dice: " + dato);

                    // Detecta cualquier mensaje del Arduino que contenga la palabra PRESIONADO o Pin 2
                    if (dato.Contains("PRESIONADO") || dato.Contains("Pin 2"))
                    {
                        activarPorSensor = true;
                    }
                }
            }
            catch (System.Exception)
            {
                // Ignoramos excepciones de timeout para que el juego no se trabe
            }
        }

        // Si se cumple alguna de las opciones, ejecutamos
        if (activarPorTeclado || activarPorSensor)
        {
            EjecutarAccion();
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("Puerto serie cerrado.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EjecutarAccion();
    }

    void EjecutarAccion()
    {
        if (mate != null)
        {
            mate.SetActive(false);
        }

        if (rufusFeliz != null && rufusMate != null)
        {
            rufusFeliz.sprite = rufusMate;
        }

        Debug.Log("¡Acción realizada: el mate desapareció y el pájaro ahora toma mate!");
    }
}