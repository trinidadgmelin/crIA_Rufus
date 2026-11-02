using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mimoInteraccion : MonoBehaviour
{
    [Header("Referencias UI / Personaje")]
    public SpriteRenderer rufusFeliz;
    public Sprite rufusMimo;
    public Slider barraProgreso;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

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
    }

    void Update()
    {
        if (ArduinoManager.Instance == null) return;

        bool activarPorTeclado = ArduinoManager.Instance.teclaBPresionada;
        bool activarPorSensor = ArduinoManager.Instance.sensorManoDetectado;

        if (activarPorTeclado || (activarPorSensor && Time.time - tiempoUltimoAumento > cooldownAumento))
        {
            AumentarProgreso();
        }
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
        necesidadesUI ui = necesidadesUI.Instance;
        if (ui != null && rufusMimo != null)
        {
            ui.MostrarSpriteTemporal(rufusMimo, segundosVisible);
        }

        // Reiniciar timer de Caricias y ponerlo a correr inmediatamente de nuevo
        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.quiereCaricias = false;
            timerNecesidades.Instance.colaNecesidades.Remove("Caricias"); // <--- Quitamos de la cola
            timerNecesidades.Instance.caricias.timerActual = timerNecesidades.Instance.caricias.intervalo;
            timerNecesidades.Instance.caricias.estaCorriendo = true;
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