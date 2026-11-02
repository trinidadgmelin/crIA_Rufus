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
    }

    void VerificarEscenaActual(string nombreEscena)
    {
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
}