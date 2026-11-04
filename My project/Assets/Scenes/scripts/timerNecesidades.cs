using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class timerNecesidades : MonoBehaviour
{
    public static timerNecesidades Instance;

    // Bandera global para pausar todos los timers cuando hay un pop-up abierto
    public static bool HayPopupAbierto = false;

    [System.Serializable]
    public class Necesidad
    {
        public string nombre;
        public float intervalo;
        public string escenaActivacion;
        [HideInInspector] public float timerActual;
        [HideInInspector] public bool estaCorriendo = false;

        [Header("Tiempo Límite de Urgencia")]
        public float tiempoMaximoEspera = 15f; // Tiempo antes de ir a finalEscape
        [HideInInspector] public float timerEsperaActual = 0f;
        [HideInInspector] public bool estaEsperando = false;
    }

    public Necesidad hambre = new Necesidad { nombre = "Hambre", intervalo = 20f, escenaActivacion = "escena3", tiempoMaximoEspera = 15f };
    public Necesidad dormir = new Necesidad { nombre = "Dormir", intervalo = 45f, escenaActivacion = "escena4", tiempoMaximoEspera = 15f };
    public Necesidad caricias = new Necesidad { nombre = "Caricias", intervalo = 20f, escenaActivacion = "escena5", tiempoMaximoEspera = 15f };

    // Banderas originales que usan tus scripts de interacción
    [HideInInspector] public bool tieneHambre = false;
    [HideInInspector] public bool quiereDormir = false;
    [HideInInspector] public bool quiereCaricias = false;

    // Cola de espera para las necesidades activas
    [HideInInspector] public List<string> colaNecesidades = new List<string>();

    // Contadores de cuántas veces se resolvió cada necesidad
    [HideInInspector] public int contadorHambre = 0;
    [HideInInspector] public int contadorDormir = 0;
    [HideInInspector] public int contadorCaricias = 0;
    public int metaPorNecesidad = 3; // Cantidad requerida para el final feliz

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
        // Si hay un pop-up abierto, congelamos todos los timers del juego
        if (HayPopupAbierto) return;

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
                hambre.estaEsperando = true;
                hambre.timerEsperaActual = hambre.tiempoMaximoEspera;

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
                dormir.estaEsperando = true;
                dormir.timerEsperaActual = dormir.tiempoMaximoEspera;

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
                caricias.estaEsperando = true;
                caricias.timerEsperaActual = caricias.tiempoMaximoEspera;

                if (!colaNecesidades.Contains("Caricias")) colaNecesidades.Add("Caricias");

                Debug.Log("¡Rufus quiere caricias!");
                AvisarUI();
            }
        }

        // --- GESTIÓN DE TIEMPO DE ESPERA / URGENCIA DE LA NECESIDAD EN CURSO ---
        if (colaNecesidades.Count > 0 && !esperandoMimoFinal)
        {
            string necesidadEnFoco = colaNecesidades[0];
            Necesidad necActiva = ObtenerNecesidadPorNombre(necesidadEnFoco);

            if (necActiva != null && necActiva.estaEsperando)
            {
                necActiva.timerEsperaActual -= Time.deltaTime;

                if (necActiva.timerEsperaActual <= 0)
                {
                    Debug.Log("¡Se agotó el tiempo de respuesta en la cola! Disparando escape...");
                    ActivarFinalEscape();
                }
            }
        }
    }

    Necesidad ObtenerNecesidadPorNombre(string nombre)
    {
        if (nombre == "Hambre") return hambre;
        if (nombre == "Dormir") return dormir;
        if (nombre == "Caricias") return caricias;
        return null;
    }

    void VerificarEscenaActual(string nombreEscena)
    {
        if (esperandoMimoFinal) return;

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
        if (necesidadesUI.Instance != null)
        {
            necesidadesUI.Instance.ActualizarVisuales();
        }
    }

    // Método que llaman tus interacciones cuando el jugador resuelve una necesidad
    public void ResolverNecesidad(string nombreNecesidad)
    {
        if (esperandoMimoFinal)
        {
            if (nombreNecesidad == "Caricias")
            {
                Debug.Log("¡Mimo final completado! Yendo a final feliz...");
                ActivarFinalFeliz();
            }
            return;
        }

        if (colaNecesidades.Contains(nombreNecesidad))
        {
            colaNecesidades.Remove(nombreNecesidad);
            Necesidad nec = ObtenerNecesidadPorNombre(nombreNecesidad);

            if (nec != null)
            {
                nec.estaEsperando = false;
                nec.timerActual = nec.intervalo;
                nec.estaCorriendo = true;
            }

            if (nombreNecesidad == "Hambre")
            {
                tieneHambre = false;
                contadorHambre++;
            }
            else if (nombreNecesidad == "Dormir")
            {
                quiereDormir = false;
                contadorDormir++;
            }
            else if (nombreNecesidad == "Caricias")
            {
                quiereCaricias = false;
                contadorCaricias++;
            }

            AvisarUI();

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

        hambre.estaCorriendo = false;
        dormir.estaCorriendo = false;
        caricias.estaCorriendo = false;

        tieneHambre = false;
        quiereDormir = false;
        quiereCaricias = false;

        colaNecesidades.Add("Caricias");
        AvisarUI();
        Debug.Log("¡Todas las necesidades completadas! Se requiere el Mimo final.");
    }

    public void ActivarFinalEscape()
    {
        SceneManager.LoadScene("finalEscape");
    }

    public void ActivarFinalFeliz()
    {
        SceneManager.LoadScene("finalFeliz");
    }
}