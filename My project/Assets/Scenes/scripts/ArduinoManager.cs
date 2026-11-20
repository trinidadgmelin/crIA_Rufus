using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;

public class ArduinoManager : MonoBehaviour
{
    public static ArduinoManager Instance;

    [Header("Configuración Arduino")]
    public int baudRate = 9600;
    private SerialPort puerto;
    private bool conectado = false;

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
        Debug.Log("Arduino creado en escena: " + SceneManager.GetActiveScene().name);

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
                prueba.ReadTimeout = 50;
                prueba.WriteTimeout = 50;
                prueba.Open();

                Thread.Sleep(2000); // Esperar reinicio del Arduino

                prueba.WriteLine("PING");
                string respuesta = prueba.ReadLine().Trim();

                if (respuesta == "ARDUINO_OK")
                {
                    puerto = prueba;
                    puerto.ReadTimeout = 20;
                    conectado = true;
                    Debug.Log("<color=green>¡Arduino conectado exitosamente en: " + nombrePuerto + "!</color>");
                    return;
                }

                prueba.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("No se pudo verificar " + nombrePuerto + ": " + e.Message);
            }
        }

        Debug.LogWarning("No se encontró ningún Arduino respondiendo al PING.");
    }

    void Update()
    {
        // Reiniciar estados por frame (los botones tipo pulso se limpian acá)
        sensorManoDetectado = false;
        botonIzquierdaPresionado = false;
        botonSeleccionarPresionado = false;
        botonDerechaPresionado = false;

        teclaBPresionada = Input.GetKeyDown(KeyCode.B);

        // Si está conectado, leemos todos los bytes acumulados en el puerto serie
        if (conectado && puerto != null && puerto.IsOpen)
        {
            try
            {
                while (puerto.BytesToRead > 0)
                {
                    string dato = puerto.ReadLine().Trim();

                    // Identificamos y registramos cada estado con Debug.Log usando tus etiquetas exactas
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

                    if (dato == "PIN_3_ACTIVADO")
                    {
                        sensorNidoPresionado = true;
                        Debug.Log("<color=magenta>[Arduino] Pin 3 ACTIVADO (Nido)</color>");
                    }
                    else if (dato == "PIN_3_DESCONECTADO")
                    {
                        sensorNidoPresionado = false;
                        Debug.Log("<color=magenta>[Arduino] Pin 3 DESCONECTADO (Nido)</color>");
                    }

                    if (dato == "MANO_CERCA")
                    {
                        sensorManoDetectado = true;
                        Debug.Log("<color=yellow>[Arduino] Ultrasonico: Mano cerca detectada (3-20 cm)</color>");
                    }

                    if (dato == "FLECHA_IZQ")
                    {
                        botonIzquierdaPresionado = true;
                        Debug.Log("<color=orange>[Arduino] Botón físico: FLECHA_IZQ presionado</color>");
                    }

                    if (dato == "BOTON_SEL")
                    {
                        botonSeleccionarPresionado = true;
                        Debug.Log("<color=orange>[Arduino] Botón físico: BOTON_SEL presionado</color>");
                    }

                    if (dato == "FLECHA_DER")
                    {
                        botonDerechaPresionado = true;
                        Debug.Log("<color=orange>[Arduino] Botón físico: FLECHA_DER presionado</color>");
                    }
                }
            }
            catch (System.Exception) { }
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