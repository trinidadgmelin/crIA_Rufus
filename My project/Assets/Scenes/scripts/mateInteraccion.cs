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
        // Tecla E o Sensor de Arduino
        bool activarPorTeclado = Input.GetKeyDown(KeyCode.E);
        bool activarPorSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorMatePresionado;

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

        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.ResolverNecesidad("Hambre");
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