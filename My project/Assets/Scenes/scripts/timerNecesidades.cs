using UnityEngine;
using UnityEngine.SceneManagement;

public class timerNecesidades : MonoBehaviour
{
    public static timerNecesidades Instance;

    [System.Serializable]
    public class Necesidad
    {
        public string nombre;
        public float intervalo;
        public string escenaActivacion;
        [HideInInspector] public float timerActual;
        [HideInInspector] public bool estaCorriendo = false;
    }

    // Definimos las 3 necesidades con sus valores fijos directamente
    public Necesidad hambre = new Necesidad { nombre = "Hambre", intervalo = 20f, escenaActivacion = "escena3" };
    public Necesidad dormir = new Necesidad { nombre = "Dormir", intervalo = 45f, escenaActivacion = "escena4" };
    public Necesidad caricias = new Necesidad { nombre = "Caricias", intervalo = 20f, escenaActivacion = "escena5" };

    // Estados globales que persisten
    [HideInInspector] public bool tieneHambre = false;
    [HideInInspector] public bool quiereDormir = false;
    [HideInInspector] public bool quiereCaricias = false;

    private string escenaAnterior = "";

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

    void Start()
    {
        // Inicializamos los timers con sus intervalos establecidos
        hambre.timerActual = hambre.intervalo;
        dormir.timerActual = dormir.intervalo;
        caricias.timerActual = caricias.intervalo;

        VerificarEscenaActual(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        // Si cambiamos de escena, verificamos si alguna debe empezar a contar
        if (escenaActual != escenaAnterior)
        {
            VerificarEscenaActual(escenaActual);
            escenaAnterior = escenaActual;
        }

        // 1. HAMBRE
        if (hambre.estaCorriendo)
        {
            hambre.timerActual -= Time.deltaTime;
            if (hambre.timerActual <= 0)
            {
                tieneHambre = true;
                hambre.estaCorriendo = false;
                Debug.Log("¡A Rufus le da hambre!");
                AvisarUI();
            }
        }

        // 2. DORMIR
        if (dormir.estaCorriendo)
        {
            dormir.timerActual -= Time.deltaTime;
            if (dormir.timerActual <= 0)
            {
                quiereDormir = true;
                dormir.estaCorriendo = false;
                Debug.Log("¡Rufus quiere dormir!");
                AvisarUI();
            }
        }

        // 3. CARICIAS
        if (caricias.estaCorriendo)
        {
            caricias.timerActual -= Time.deltaTime;
            if (caricias.timerActual <= 0)
            {
                quiereCaricias = true;
                caricias.estaCorriendo = false;
                Debug.Log("¡Rufus quiere caricias!");
                AvisarUI();
            }
        }
    }

    void VerificarEscenaActual(string nombreEscena)
    {
        // Activar Hambre en escena3
        if (nombreEscena == hambre.escenaActivacion && !tieneHambre && !hambre.estaCorriendo)
        {
            hambre.estaCorriendo = true;
            Debug.Log("Timer de Hambre iniciado en: " + nombreEscena);
        }

        // Activar Dormir en escena4
        if (nombreEscena == dormir.escenaActivacion && !quiereDormir && !dormir.estaCorriendo)
        {
            dormir.estaCorriendo = true;
            Debug.Log("Timer de Dormir iniciado en: " + nombreEscena);
        }

        // Activar Caricias en escena5
        if (nombreEscena == caricias.escenaActivacion && !quiereCaricias && !caricias.estaCorriendo)
        {
            caricias.estaCorriendo = true;
            Debug.Log("Timer de Caricias iniciado en: " + nombreEscena);
        }
    }

    void AvisarUI()
    {
        necesidadesUI uiActual = FindObjectOfType<necesidadesUI>();
        if (uiActual != null)
        {
            uiActual.ActualizarVisuales();
        }
    }
}