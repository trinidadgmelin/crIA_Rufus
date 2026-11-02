using UnityEngine;
using System.IO.Ports;

public class ArduinoManager : MonoBehaviour
{
    public static ArduinoManager Instance;

    [Header("Configuración Arduino")]
    public string puertoSerial = "COM5";
    public int baudRate = 9600;
    private SerialPort stream;

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
        try
        {
            stream = new SerialPort(puertoSerial, baudRate);
            stream.ReadTimeout = 50;
            stream.Open();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo abrir el puerto serial: " + e.Message);
        }
    }

    void Update()
    {
        // Reiniciar estados por frame
        sensorManoDetectado = false;
        sensorMatePresionado = false;
        sensorNidoPresionado = false;
        teclaBPresionada = Input.GetKeyDown(KeyCode.B);

        if (stream != null && stream.IsOpen)
        {
            try
            {
                if (stream.BytesToRead > 0)
                {
                    string dato = stream.ReadLine().Trim();

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

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }
    }
}