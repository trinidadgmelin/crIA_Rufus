using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class mateInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject mate;
    public SpriteRenderer rufusFeliz;
    public Sprite rufusMate;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    void Update()
    {
        if (ArduinoManager.Instance == null) return;

        bool activarPorTeclado = ArduinoManager.Instance.teclaBPresionada;
        bool activarPorSensor = ArduinoManager.Instance.sensorMatePresionado;

        if (activarPorTeclado || activarPorSensor)
        {
            EjecutarAccion();
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

        necesidadesUI ui = necesidadesUI.Instance;
        if (ui != null && rufusMate != null)
        {
            ui.MostrarSpriteTemporal(rufusMate, segundosVisible);
        }

        // Reiniciar timer de Hambre y correr inmediatamente
        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.tieneHambre = false;
            timerNecesidades.Instance.colaNecesidades.Remove("Hambre"); // <--- Quitamos de la cola
            timerNecesidades.Instance.hambre.timerActual = timerNecesidades.Instance.hambre.intervalo;
            timerNecesidades.Instance.hambre.estaCorriendo = true;
        }

        StartCoroutine(RestaurarMate());
    }

    IEnumerator RestaurarMate()
    {
        yield return new WaitForSeconds(segundosVisible);

        if (mate != null)
        {
            mate.SetActive(true);
        }
    }
}