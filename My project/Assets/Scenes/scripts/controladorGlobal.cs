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

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    private bool durmiendoEnCurso = false;
    private float tiempoFinSpriteTemporal = 0f;

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
        // Si hay un pop-up abierto, la UI está bloqueada o ya estamos durmiendo, ignoramos inputs
        if (timerNecesidades.HayPopupAbierto || durmiendoEnCurso) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

        // Detección por Teclado (1, 2, 3) y Sensores de Arduino
        bool usarMateTeclado = Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
        bool usarMateSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorMatePresionado;

        bool usarMimoTeclado = Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        bool usarMimoSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorManoDetectado;

        bool usarDormirTeclado = Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);
        bool usarDormirSensor = (ArduinoManager.Instance != null) && ArduinoManager.Instance.sensorNidoPresionado;

        // 1. ACCIÓN: HAMBRE (Mate) -> Tecla 1 o Sensor Pin 2
        if (usarMateTeclado || usarMateSensor)
        {
            EjecutarAccionGlobal("Hambre", rufusMate);
        }

        // 2. ACCIÓN: CARICIAS (Mimo) -> Tecla 2 o Sensor Mano
        if (usarMimoTeclado || usarMimoSensor)
        {
            EjecutarAccionGlobal("Caricias", rufusMimo);
        }

        // 3. ACCIÓN: DORMIR (Nido) -> Tecla 3 o Sensor Pin 3 (bloqueado mientras haya un sprite temporal activo)
        bool spriteTemporalActivo = Time.time < tiempoFinSpriteTemporal;
        if (!spriteTemporalActivo && (usarDormirTeclado || usarDormirSensor))
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

        // 1. Forzamos el bloqueo de la UI y hitboxes inmediatamente al iniciar la acción
        if (necesidadesUI.Instance != null)
        {
            necesidadesUI.Instance.ForzarBloqueo(true);
        }

        if (timerNecesidades.Instance != null)
        {
            timerNecesidades.Instance.ResolverNecesidad("Dormir");
        }

        // Si no estamos en la escena de dormir, la cargamos de forma asíncrona
        if (SceneManager.GetActiveScene().name != nombreEscenaDormir)
        {
            AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscenaDormir);
            while (!carga.isDone)
            {
                yield return null;
            }
            yield return null; // Esperamos un frame extra para inicializar referencias
        }

        GameObject objetoRufus = GameObject.Find("Rufus");
        GameObject sueñoAd = GameObject.Find("sueñoAd");

        SpriteRenderer fondoSR = BuscarFondoEscena();
        Sprite fondoOriginal = (fondoSR != null) ? fondoSR.sprite : null;

        if (objetoRufus != null) objetoRufus.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        // Mantenemos el sprite dormido durante todo el tiempo visible
        float tiempoRestante = segundosVisible;
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

        // 2. Desbloqueamos la UI y las hitboxes al finalizar la animación
        if (necesidadesUI.Instance != null)
        {
            necesidadesUI.Instance.ForzarBloqueo(false);
        }

        durmiendoEnCurso = false;
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