using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.Ports;

public class mimoInteraccion : MonoBehaviour
{
    [Header("Referencias UI / Personaje")]
    public SpriteRenderer rufusFeliz;
    public Sprite rufusMimo;
    public Slider barraProgreso;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    [Header("Configuración Arduino (Ultrasónico)")]
    public string puertoSerial = "COM5";
    public int baudRate = 9600;
    private SerialPort stream;

    private float progresoActual = 0f;
    private float tiempoUltimoAumento = 0f;
    private float cooldownAumento = 0.8f;

    void Start()
    {
        if (barraProgreso != null)
        {
            barraProgreso.minValue = 0f;
            barraProgreso.maxValue = 1f;
            barraProgreso.value = 0f;
        }

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
                    if (dato.Contains("Mano") || dato.Contains("cerca"))
                    {
                        activarPorSensor = true;
                    }
                }
            }
            catch (System.Exception) { }
        }

        if (activarPorTeclado || (activarPorSensor && Time.time - tiempoUltimoAumento > cooldownAumento))
        {
            AumentarProgreso();
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen) stream.Close();
    }

    void AumentarProgreso()
    {
        if (progresoActual >= 1f) return;

        tiempoUltimoAumento = Time.time;
        progresoActual += 0.25f;

        if (barraProgreso != null) barraProgreso.value = progresoActual;

        if (progresoActual >= 1f)
        {
            EjecutarAccionFinal();
        }
    }

    void EjecutarAccionFinal()
    {
        necesidadesUI ui = FindObjectOfType<necesidadesUI>();
        if (ui != null && rufusMimo != null)
        {
            ui.MostrarSpriteTemporal(rufusMimo, segundosVisible);
        }

        // Reiniciar timer de Caricias
        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.quiereCaricias = false;
            timerNecesidades.Instance.caricias.timerActual = timerNecesidades.Instance.caricias.intervalo;
            timerNecesidades.Instance.caricias.estaCorriendo = false;
        }

        StartCoroutine(ResetearBarra());
    }

    IEnumerator ResetearBarra()
    {
        yield return new WaitForSeconds(segundosVisible);
        progresoActual = 0f;
        if (barraProgreso != null) barraProgreso.value = 0f;
    }
}