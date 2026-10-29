using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO.Ports;

public class nidoInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public Image rufusFeliz; // El objeto de Rufus que se va a eliminar

    [Header("Configuración del Fondo")]
    public Image fondoImage;         // Componente Image del fondo
    public Sprite nuevoFondoSprite;  // Nuevo sprite para el fondo

    [Header("Configuración Arduino")]
    public string puertoSerial = "COM5"; // Asegurate que coincida con tu puerto
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
        // 1. Detección por teclado (Tecla B)
        bool activarPorTeclado = Input.GetKeyDown(KeyCode.B);

        bool activarPorSensor = false;

        // 2. Detección por Puerto Serie (Arduino - Pin 3)
        if (stream != null && stream.IsOpen)
        {
            try
            {
                if (stream.BytesToRead > 0)
                {
                    string dato = stream.ReadLine().Trim();

                    Debug.Log("Arduino dice: " + dato);

                    // Detecta el mensaje específico del Pin 3 cuando se presiona
                    if (dato.Contains("PRESIONADO") && dato.Contains("Pin 3"))
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

        // Si se presiona la tecla B o se activa el sensor del Pin 3, ejecutamos
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
        // Elimina por completo el objeto de Rufus de la escena
        if (rufusFeliz != null)
        {
            Destroy(rufusFeliz.gameObject);
        }

        // Cambia el sprite del fondo de la UI
        if (fondoImage != null && nuevoFondoSprite != null)
        {
            fondoImage.sprite = nuevoFondoSprite;
        }

        Debug.Log("¡Acción realizada: Rufus fue eliminado y se cambió el fondo!");
    }
}