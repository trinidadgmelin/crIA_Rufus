using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    public Necesidad hambre = new Necesidad { nombre = "Hambre", intervalo = 20f, escenaActivacion = "escena3" };
    public Necesidad dormir = new Necesidad { nombre = "Dormir", intervalo = 45f, escenaActivacion = "escena4" };
    public Necesidad caricias = new Necesidad { nombre = "Caricias", intervalo = 20f, escenaActivacion = "escena5" };

    [HideInInspector] public bool tieneHambre = false;
    [HideInInspector] public bool quiereDormir = false;
    [HideInInspector] public bool quiereCaricias = false;

    // Cola de espera para las necesidades activas (guarda el orden de llegada)
    [HideInInspector] public List<string> colaNecesidades = new List<string>();

    [Header("Condiciones de Victoria y Derrota")]
    public float tiempoExpiracion = 15f; // Segundos antes de ir a finalEscape
    private float timerExpiracionActual = 0f;
    private bool inspeccionandoExpiracion = false;

    // Contadores de cuántas veces se resolvió cada necesidad
    [HideInInspector] public int contadorHambre = 0;
    [HideInInspector] public int contadorDormir = 0;
    [HideInInspector] public int contadorCaricias = 0;
    public int metaPorNecesidad = 3; // Cantidad requerida de cada una para el final feliz

    private bool esperandoMimoFinal = false;
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
        hambre.timerActual = hambre.intervalo;
        dormir.timerActual = dormir.intervalo;
        caricias.timerActual = caricias.intervalo;

        VerificarEscenaActual(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

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

                if (!colaNecesidades.Contains("Hambre")) colaNecesidades.Add("Hambre");

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

                if (!colaNecesidades.Contains("Dormir")) colaNecesidades.Add("Dormir");

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

                if (!colaNecesidades.Contains("Caricias")) colaNecesidades.Add("Caricias");

                Debug.Log("¡Rufus quiere caricias!");
                AvisarUI();
            }
        }

        // --- CONDICIÓN DE DERROTA (TIMER DE 15 SEGUNDOS DE EXPIRACIÓN) ---
        if (colaNecesidades.Count > 0 && !esperandoMimoFinal)
        {
            if (!inspeccionandoExpiracion)
            {
                inspeccionandoExpiracion = true;
                timerExpiracionActual = tiempoExpiracion;
            }

            timerExpiracionActual -= Time.deltaTime;
            if (timerExpiracionActual <= 0)
            {
                Debug.Log("¡Se agotó el tiempo de respuesta en la cola! Disparando escape...");
                ActivarFinalEscape();
            }
        }
        else
        {
            inspeccionandoExpiracion = false;
        }
    }

    void VerificarEscenaActual(string nombreEscena)
    {
        if (esperandoMimoFinal) return; // Si ya cumplimos todo y esperamos el mimo final, no activamos más bucles

        if (nombreEscena == hambre.escenaActivacion && !tieneHambre && !hambre.estaCorriendo)
        {
            hambre.estaCorriendo = true;
        }

        if (nombreEscena == dormir.escenaActivacion && !quiereDormir && !dormir.estaCorriendo)
        {
            dormir.estaCorriendo = true;
        }

        if (nombreEscena == caricias.escenaActivacion && !quiereCaricias && !caricias.estaCorriendo)
        {
            caricias.estaCorriendo = true;
        }
    }

    void AvisarUI()
    {
        necesidadesUI uiActual = necesidadesUI.Instance;
        if (uiActual != null)
        {
            uiActual.ActualizarVisuales();
        }
    }

    // Método que llaman tus interacciones cuando el jugador resuelve una necesidad
    public void ResolverNecesidad(string nombreNecesidad)
    {
        if (esperandoMimoFinal)
        {
            if (nombreNecesidad == "Caricias") // O el nombre que le des al último Mimo
            {
                Debug.Log("¡Mimo final completado! Yendo a final feliz...");
                ActivarFinalFeliz();
            }
            return;
        }

        if (colaNecesidades.Contains(nombreNecesidad))
        {
            colaNecesidades.Remove(nombreNecesidad);

            // Reiniciamos su respectivo timer e incrementamos su contador de progreso
            if (nombreNecesidad == "Hambre")
            {
                tieneHambre = false;
                contadorHambre++;
                hambre.timerActual = hambre.intervalo;
                hambre.estaCorriendo = true;
            }
            else if (nombreNecesidad == "Dormir")
            {
                quiereDormir = false;
                contadorDormir++;
                dormir.timerActual = dormir.intervalo;
                dormir.estaCorriendo = true;
            }
            else if (nombreNecesidad == "Caricias")
            {
                quiereCaricias = false;
                contadorCaricias++;
                caricias.timerActual = caricias.intervalo;
                caricias.estaCorriendo = true;
            }

            AvisarUI();

            // Verificamos si ya cumplimos la meta de las 3 de cada tipo
            if (contadorHambre >= metaPorNecesidad && contadorDormir >= metaPorNecesidad && contadorCaricias >= metaPorNecesidad)
            {
                DispararMimoFinal();
            }
        }
    }

    void DispararMimoFinal()
    {
        esperandoMimoFinal = true;
        colaNecesidades.Clear();

        // Apagamos los timers normales para que no sigan apareciendo necesidades
        hambre.estaCorriendo = false;
        dormir.estaCorriendo = false;
        caricias.estaCorriendo = false;

        // Metemos el requerimiento final a la cola
        colaNecesidades.Add("Caricias"); // O la etiqueta especial para el Mimo final
        AvisarUI();
        Debug.Log("¡Todas las necesidades completadas! Se requiere el Mimo final.");
    }

    // Método para disparar el Final de Escape (por tiempo agotado)
    public void ActivarFinalEscape()
    {
        SceneManager.LoadScene("finalEscape");
    }

    // Método para disparar el Final Feliz (por completar todas las necesidades)
    public void ActivarFinalFeliz()
    {
        SceneManager.LoadScene("finalFeliz");
    }
}