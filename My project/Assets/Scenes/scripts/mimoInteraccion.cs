using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class mimoInteraccion : MonoBehaviour
{
    [Header("Referencias UI / Personaje")]
    public Image rufusFeliz;          // El componente Image de Rufus
    public Sprite rufusMimo;          // El nuevo sprite que tendrá Rufus al llenarse la barra
    public Slider barraProgreso;      // Arrastrá acá tu Slider de la UI

    [Header("Configuración Arduino (Ultrasónico)")]
    public string puertoSerial = "COM5";
    public int baudRate = 9600;
    private SerialPort stream;

    private float progresoActual = 0f; // Lleva la cuenta (va de 0.0 a 1.0)
    private float tiempoUltimoAumento = 0f;
    private float cooldownAumento = 0.8f; // Tiempo mínimo entre cada aumento automático por sensor

    void Start()
    {
        // Configuramos la barra para que empiece vacía (0) y su máximo sea 1
        if (barraProgreso != null)
        {
            barraProgreso.minValue = 0f;
            barraProgreso.maxValue = 1f;
            barraProgreso.value = 0f;
        }

        // Inicializamos el puerto serie para escuchar al Arduino
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

        // 2. Detección por Puerto Serie (Arduino - Sensor Ultrasónico)
        if (stream != null && stream.IsOpen)
        {
            try
            {
                if (stream.BytesToRead > 0)
                {
                    string dato = stream.ReadLine().Trim();

                    // Esto te muestra en la consola exactamente qué está llegando desde el Arduino
                    Debug.Log("Arduino dice: [" + dato + "]");

                    // Condición flexible: detecta si el texto contiene "Mano" o "cerca"
                    if (dato.Contains("Mano") || dato.Contains("cerca"))
                    {
                        activarPorSensor = true;
                    }
                }
            }
            catch (System.Exception)
            {
                // Ignoramos excepciones de timeout
            }
        }

        // Aumenta si presionas 'B' O si el sensor detecta la mano (respetando el cooldown)
        if (activarPorTeclado || (activarPorSensor && Time.time - tiempoUltimoAumento > cooldownAumento))
        {
            AumentarProgreso();
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

    void AumentarProgreso()
    {
        if (progresoActual >= 1f) return;

        tiempoUltimoAumento = Time.time;
        progresoActual += 0.25f;

        if (barraProgreso != null)
        {
            barraProgreso.value = progresoActual;
        }

        Debug.Log("Progreso de la barra: " + (progresoActual * 100) + "%");

        if (progresoActual >= 1f)
        {
            EjecutarAccionFinal();
        }
    }

    void EjecutarAccionFinal()
    {
        if (rufusFeliz != null && rufusMimo != null)
        {
            rufusFeliz.sprite = rufusMimo;
        }

        Debug.Log("¡Acción completada: barra llena y sprite de Rufus cambiado a Mimo!");
    }
}