using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Text;
using UnityEngine.SceneManagement;

public class ArduinoManager : MonoBehaviour
{
    public static ArduinoManager Instance;

    [Header("Configuración Arduino")]
    public int baudRate = 9600;
    private SerialPort puerto;
    private bool conectado = false;
    private StringBuilder bufferLectura = new StringBuilder();

    // Estados leídos
    [HideInInspector] public bool sensorManoDetectado = false;
    [HideInInspector] public bool sensorMatePresionado = false;
    [HideInInspector] public bool sensorNidoPresionado = false;

    // Estados para los botones físicos de navegación
    [HideInInspector] public bool botonIzquierdaPresionado = false;
    [HideInInspector] public bool botonSeleccionarPresionado = false;
    [HideInInspector] public bool botonDerechaPresionado = false;

    [HideInInspector] public bool teclaBPresionada = false;

    void Awake()
    {
        Debug.Log("ArduinoManager Awake en escena: " + SceneManager.GetActiveScene().name);

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
        Debug.Log("[Arduino] Puertos detectados: " + (puertos.Length == 0 ? "ninguno" : string.Join(", ", puertos)));

        foreach (string nombrePuerto in puertos)
        {
            SerialPort prueba = null;
            try
            {
                prueba = new SerialPort(nombrePuerto, baudRate);
                prueba.NewLine = "\n";
                prueba.ReadTimeout = 500;
                prueba.WriteTimeout = 500;
                prueba.DtrEnable = true;
                prueba.RtsEnable = true;
                prueba.Open();

                Thread.Sleep(2000); // Esperar reinicio del Arduino tras abrir el puerto
                prueba.DiscardInBuffer();

                prueba.WriteLine("PING");

                bool respondioOk = false;
                for (int intento = 0; intento < 5; intento++)
                {
                    try
                    {
                        string linea = prueba.ReadLine().Trim();
                        Debug.Log("[Arduino] Respuesta en " + nombrePuerto + ": '" + linea + "'");
                        if (linea == "ARDUINO_OK")
                        {
                            respondioOk = true;
                            break;
                        }
                    }
                    catch (System.TimeoutException)
                    {
                        // Reintentar dentro de la ventana de espera
                    }
                }

                if (respondioOk)
                {
                    puerto = prueba;
                    puerto.ReadTimeout = 20;
                    conectado = true;
                    Debug.Log("<color=green>[Arduino] Conectado exitosamente en: " + nombrePuerto + "</color>");
                    return;
                }

                prueba.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[Arduino] Error verificando " + nombrePuerto + ": " + e.Message);
                if (prueba != null && prueba.IsOpen)
                {
                    prueba.Close();
                }
            }
        }

        Debug.LogWarning("[Arduino] No se encontró ningún Arduino respondiendo a PING. Recordá cerrar el Monitor Serie del Arduino IDE.");
    }

    void Update()
    {
        // Reiniciar pulsos por frame
        sensorManoDetectado = false;
        botonIzquierdaPresionado = false;
        botonSeleccionarPresionado = false;
        botonDerechaPresionado = false;

        teclaBPresionada = Input.GetKeyDown(KeyCode.B);

        if (!conectado || puerto == null || !puerto.IsOpen) return;

        try
        {
            if (puerto.BytesToRead > 0)
            {
                string entrante = puerto.ReadExisting();
                bufferLectura.Append(entrante);

                string contenido = bufferLectura.ToString();
                int indiceSalto;

                while ((indiceSalto = contenido.IndexOf('\n')) >= 0)
                {
                    string dato = contenido.Substring(0, indiceSalto).Trim();
                    contenido = contenido.Substring(indiceSalto + 1);

                    if (!string.IsNullOrEmpty(dato))
                    {
                        ProcesarDato(dato);
                    }
                }

                bufferLectura.Clear();
                bufferLectura.Append(contenido);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[Arduino] Error leyendo puerto serie: " + e.Message);
        }
    }

    void ProcesarDato(string dato)
    {
        if (dato == "PIN_2_ACTIVADO")
        {
            sensorMatePresionado = true;
            Debug.Log("<color=cyan>[Arduino] Pin 2 ACTIVADO (Mate)</color>");
        }
        else if (dato == "PIN_2_DESCONECTADO")
        {
            sensorMatePresionado = false;
            Debug.Log("<color=cyan>[Arduino] Pin 2 DESCONECTADO (Mate)</color>");
        }
        else if (dato == "PIN_3_ACTIVADO")
        {
            sensorNidoPresionado = true;
            Debug.Log("<color=magenta>[Arduino] Pin 3 ACTIVADO (Nido)</color>");
        }
        else if (dato == "PIN_3_DESCONECTADO")
        {
            sensorNidoPresionado = false;
            Debug.Log("<color=magenta>[Arduino] Pin 3 DESCONECTADO (Nido)</color>");
        }
        else if (dato == "MANO_CERCA")
        {
            sensorManoDetectado = true;
            Debug.Log("<color=yellow>[Arduino] Ultrasonico: Mano cerca detectada (3-20 cm)</color>");
        }
        else if (dato == "FLECHA_IZQ")
        {
            botonIzquierdaPresionado = true;
            Debug.Log("<color=orange>[Arduino] Botón físico: FLECHA_IZQ presionado</color>");
        }
        else if (dato == "BOTON_SEL")
        {
            botonSeleccionarPresionado = true;
            Debug.Log("<color=orange>[Arduino] Botón físico: BOTON_SEL presionado</color>");
        }
        else if (dato == "FLECHA_DER")
        {
            botonDerechaPresionado = true;
            Debug.Log("<color=orange>[Arduino] Botón físico: FLECHA_DER presionado</color>");
        }
    }

    public void EnviarDato(string dato)
    {
        if (conectado && puerto != null && puerto.IsOpen)
        {
            puerto.WriteLine(dato);
            Debug.Log("[Arduino] Dato enviado: " + dato);
        }
    }

    void OnApplicationQuit()
    {
        if (puerto != null && puerto.IsOpen)
        {
            puerto.Close();
            Debug.Log("[Arduino] Puerto serie cerrado correctamente.");
        }
    }
}