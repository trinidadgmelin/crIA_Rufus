using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO.Ports;

public class mateInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject mate; // El objeto del mate
    public SpriteRenderer rufusFeliz;
    public Sprite rufusMate;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    [Header("Configuración Arduino")]
    public string puertoSerial = "COM5";
    public int baudRate = 9600;
    private SerialPort stream;

    void Start()
    {
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
                    if (dato.Contains("PRESIONADO") || dato.Contains("Pin 2"))
                    {
                        activarPorSensor = true;
                    }
                }
            }
            catch (System.Exception) { }
        }

        if (activarPorTeclado || activarPorSensor)
        {
            EjecutarAccion();
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen) stream.Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EjecutarAccion();
    }

    void EjecutarAccion()
    {
        // 1. Desactivar el objeto del mate para que desaparezca visualmente
        if (mate != null)
        {
            mate.SetActive(false);
        }

        // 2. Mostrar el sprite temporal de Rufus tomando mate a través de la UI 
        // (Esto automáticamente oculta los carteles de advertencia para que no estorben)
        necesidadesUI ui = FindObjectOfType<necesidadesUI>();
        if (ui != null && rufusMate != null)
        {
            ui.MostrarSpriteTemporal(rufusMate, segundosVisible);
        }

        // 3. Reiniciar el timer de Hambre
        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.tieneHambre = false;
            timerNecesidades.Instance.hambre.timerActual = timerNecesidades.Instance.hambre.intervalo;
            timerNecesidades.Instance.hambre.estaCorriendo = false;
        }

        StartCoroutine(RestaurarMate());
    }

    IEnumerator RestaurarMate()
    {
        yield return new WaitForSeconds(segundosVisible);

        // 4. Vuelve a aparecer el mate pasado el tiempo
        if (mate != null)
        {
            mate.SetActive(true);
        }
    }
}