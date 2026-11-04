using UnityEngine;
using System.Collections;

public class mimoInteraccion : MonoBehaviour
{
    [Header("Referencias UI / Personaje")]
    public SpriteRenderer rufusFeliz;
    public Sprite rufusMimo;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    private float tiempoUltimoAumento = 0f;
    private float cooldownAumento = 0.8f;

    void Update()
    {
        // Tecla E del teclado
        bool activarPorTeclado = Input.GetKeyDown(KeyCode.E);

        // Sensor de Arduino
        bool activarPorSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorManoDetectado;

        if (activarPorTeclado || (activarPorSensor && Time.time - tiempoUltimoAumento > cooldownAumento))
        {
            EjecutarAccionFinal();
        }
    }

    void EjecutarAccionFinal()
    {
        tiempoUltimoAumento = Time.time;

        necesidadesUI ui = necesidadesUI.Instance;
        if (ui != null && rufusMimo != null)
        {
            ui.MostrarSpriteTemporal(rufusMimo, segundosVisible);
        }

        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.ResolverNecesidad("Caricias");
        }
    }
}