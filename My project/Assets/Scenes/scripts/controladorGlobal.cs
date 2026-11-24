using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControladorGlobalInteracciones : MonoBehaviour
{
    public static ControladorGlobalInteracciones Instance;

    public bool EstaDurmiendo => durmiendoEnCurso;

    [Header("Sprites Temporales de Interacción")]
    public Sprite rufusMate;       // Tecla 1 / Sensor Mate (Hambre)
    public Sprite rufusMimo;       // Tecla 2 / Sensor Mano (Caricias)

    [Header("Configuración Específica de Dormir")]
    public string nombreEscenaDormir = "escena4";
    public Sprite fondoDormidoSprite; // Sprite del fondo con Rufus durmiendo en el nido
    public float segundosDormir = 5f;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    private bool durmiendoEnCurso = false;
    private float tiempoFinSpriteTemporal = 0f;
    private bool mateEstabaPresionado = false;
    private bool nidoEstabaPresionado = false;

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
        }
    }

    void Update()
    {
        bool estadoMateActual = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorMatePresionado;
        bool usarMateSensor = estadoMateActual && !mateEstabaPresionado;
        mateEstabaPresionado = estadoMateActual;

        bool estadoNidoActual = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorNidoPresionado;
        bool usarNidoSensor = estadoNidoActual && !nidoEstabaPresionado;
        nidoEstabaPresionado = estadoNidoActual;

        if (timerNecesidades.HayPopupAbierto || durmiendoEnCurso) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

        bool usarMateTeclado = Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);

        bool usarMimoTeclado = Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        bool usarMimoSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorManoDetectado;

        bool usarDormirTeclado = Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);

        if (usarMateTeclado || usarMateSensor)
        {
            EjecutarAccionGlobal("Hambre", rufusMate);
        }

        if (usarMimoTeclado || usarMimoSensor)
        {
            EjecutarAccionGlobal("Caricias", rufusMimo);
        }

        bool spriteTemporalActivo = Time.time < tiempoFinSpriteTemporal;
        if (!spriteTemporalActivo && (usarDormirTeclado || usarNidoSensor))
        {
            StartCoroutine(RutinaDormirGlobal());
        }
    }

    void EjecutarAccionGlobal(string necesidad, Sprite spriteTemporal)
    {
        tiempoFinSpriteTemporal = Time.time + segundosVisible;

        if (necesidadesUI.Instance != null && spriteTemporal != null)
        {
            necesidadesUI.Instance.MostrarSpriteTemporal(spriteTemporal, segundosVisible);
        }

        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.ResolverNecesidad(necesidad);
        }

        Debug.Log("Interacción global ejecutada: " + necesidad);
    }

    IEnumerator RutinaDormirGlobal()
    {
        durmiendoEnCurso = true;
        timerNecesidades.EvaluarBloqueoYPausaGlobal();

        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.ResolverNecesidad("Dormir");
        }

        if (SceneManager.GetActiveScene().name != nombreEscenaDormir)
        {
            AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscenaDormir);
            while (!carga.isDone)
            {
                yield return null;
            }
            yield return null;
        }

        GameObject objetoRufus = GameObject.Find("Rufus");
        GameObject sueñoAd = GameObject.Find("sueñoAd");

        SpriteRenderer fondoSR = BuscarFondoEscena();
        Sprite fondoOriginal = (fondoSR != null) ? fondoSR.sprite : null;

        if (objetoRufus != null) objetoRufus.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        float tiempoRestante = segundosDormir;
        while (tiempoRestante > 0f)
        {
            if (fondoSR != null && fondoDormidoSprite != null && fondoSR.sprite != fondoDormidoSprite)
            {
                fondoSR.sprite = fondoDormidoSprite;
            }
            if (objetoRufus != null && objetoRufus.activeSelf) objetoRufus.SetActive(false);
            if (sueñoAd != null && sueñoAd.activeSelf) sueñoAd.SetActive(false);

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == nombreEscenaDormir)
        {
            if (objetoRufus != null) objetoRufus.SetActive(true);
            if (fondoSR != null && fondoOriginal != null)
            {
                fondoSR.sprite = fondoOriginal;
            }
        }

        durmiendoEnCurso = false;
        timerNecesidades.EvaluarBloqueoYPausaGlobal();
    }

    SpriteRenderer BuscarFondoEscena()
    {
        GameObject fondoObj = GameObject.Find("fondoHab");
        if (fondoObj != null)
        {
            return fondoObj.GetComponent<SpriteRenderer>();
        }

        return null;
    }
}