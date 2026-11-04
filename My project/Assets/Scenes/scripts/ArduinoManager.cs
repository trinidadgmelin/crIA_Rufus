using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoManager : MonoBehaviour
{
    public static ArduinoManager Instance;

    [Header("Configuración Arduino")]
    public int baudRate = 9600;
    private SerialPort puerto;
    private bool conectado = false;

    // Estados leídos para este frame
    [HideInInspector] public bool sensorManoDetectado = false;
    [HideInInspector] public bool sensorMatePresionado = false;
    [HideInInspector] public bool sensorNidoPresionado = false;
    [HideInInspector] public bool teclaBPresionada = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ConectarArduinoAutomatico();
    }

    void ConectarArduinoAutomatico()
    {
        string[] puertos = SerialPort.GetPortNames();

        foreach (string nombrePuerto in puertos)
        {
            try
            {
                SerialPort prueba = new SerialPort(nombrePuerto, baudRate);
                prueba.ReadTimeout = 1000;
                prueba.WriteTimeout = 1000;
                prueba.Open();

                // Esperar el reinicio del Arduino al abrir el puerto
                Thread.Sleep(2000);

                // Comprobar si responde con el protocolo PING
                prueba.WriteLine("PING");
                string respuesta = prueba.ReadLine().Trim();

                if (respuesta == "ARDUINO_OK")
                {
                    puerto = prueba;
                    conectado = true;
                    Debug.Log("Arduino conectado automáticamente en: " + nombrePuerto);
                    return;
                }

                prueba.Close();
            }
            catch (System.Exception e)
            {
                // Este puerto falló o no era el Arduino, pasamos al siguiente
                Debug.Log("No se pudo verificar " + nombrePuerto + ": " + e.Message);
            }
        }

        Debug.LogWarning("No se encontró ningún Arduino respondiendo al PING.");
    }

    void Update()
    {
        // Reiniciar estados por frame
        sensorManoDetectado = false;
        sensorMatePresionado = false;
        sensorNidoPresionado = false;
        teclaBPresionada = Input.GetKeyDown(KeyCode.B);

        // Si está conectado, leemos los datos que envía el Arduino
        if (conectado && puerto != null && puerto.IsOpen)
        {
            try
            {
                if (puerto.BytesToRead > 0)
                {
                    string dato = puerto.ReadLine().Trim();

                    // Identificamos según lo que envíe tu Arduino
                    if (dato.Contains("Mano") || dato.Contains("cerca"))
                    {
                        sensorManoDetectado = true;
                    }
                    if (dato.Contains("PRESIONADO") || dato.Contains("Pin 2"))
                    {
                        sensorMatePresionado = true;
                    }
                    if (dato.Contains("PRESIONADO") && dato.Contains("Pin 3"))
                    {
                        sensorNidoPresionado = true;
                    }
                }
            }
            catch (System.Exception) { }
        }
    }

    // Método opcional por si necesitas enviar datos al Arduino desde Unity
    public void EnviarDato(string dato)
    {
        if (conectado && puerto != null && puerto.IsOpen)
        {
            puerto.WriteLine(dato);
        }
    }

    void OnApplicationQuit()
    {
        if (puerto != null && puerto.IsOpen)
        {
            puerto.Close();
        }
    }
}