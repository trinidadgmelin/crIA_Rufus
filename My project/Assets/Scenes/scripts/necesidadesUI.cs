using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class necesidadesUI : MonoBehaviour
{
    public static necesidadesUI Instance;

    // Propiedad pública que indica si la UI está bloqueada (mostrando sprite temporal)
    public bool EstaBloqueado { get; private set; } = false;

    [Header("Sprite por Defecto de Rufus")]
    public SpriteRenderer rufusSpriteRenderer;
    public Sprite spriteNormal;

    [Header("Configuración de Hambre")]
    public Sprite rufusEnojo;
    public GameObject hambreAd;

    [Header("Configuración de Caricias")]
    public Sprite rufusTriste;
    public GameObject mimoAd;

    [Header("Configuración de Dormir")]
    public Sprite rufusSueño;
    public GameObject sueñoAd;

    private bool bloquearActualizacion = false;

    private Dictionary<string, Sprite> ultimosSpritesPorNecesidad = new Dictionary<string, Sprite>();

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🎯 Si cargamos una escena de final, no tocamos nada para dejar libres los gráficos de derrota/victoria
        if (scene.name.Contains("final")) return;

        ReconectarReferencias();
        ActualizarVisuales();
    }

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        if (escenaActual.Contains("final")) return;

        ReconectarReferencias();
        ActualizarVisuales();
    }

    void Update()
    {
        // 🎯 Si estamos en una escena de final, el script descansa y no pisa los gráficos de la UI
        string escenaActual = SceneManager.GetActiveScene().name;
        if (escenaActual.Contains("final")) return;

        if (!bloquearActualizacion)
        {
            ActualizarVisuales();
        }
    }

    void ReconectarReferencias()
    {
        if (rufusSpriteRenderer == null)
        {
            GameObject rufusObj = BuscarObjetoInactivo("Rufus");
            if (rufusObj != null) rufusSpriteRenderer = rufusObj.GetComponent<SpriteRenderer>();
        }

        if (hambreAd == null) hambreAd = BuscarObjetoInactivo("hambreAd");
        if (mimoAd == null) mimoAd = BuscarObjetoInactivo("mimoAd");
        if (sueñoAd == null) sueñoAd = BuscarObjetoInactivo("sueñoAd");
    }

    GameObject BuscarObjetoInactivo(string nombre)
    {
        UnityEngine.Transform[] todosLosTransform = Resources.FindObjectsOfTypeAll<UnityEngine.Transform>();

        foreach (UnityEngine.Transform t in todosLosTransform)
        {
            if (t.hideFlags == HideFlags.None && t.name == nombre && t.gameObject.scene == SceneManager.GetActiveScene())
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public void ActualizarVisuales()
    {
        timerNecesidades elTimer = timerNecesidades.Instance;
        if (elTimer == null) return;

        if (hambreAd != null) hambreAd.SetActive(false);
        if (mimoAd != null) mimoAd.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        if (elTimer.colaNecesidades.Count > 0)
        {
            string necesidadActual = elTimer.colaNecesidades[0];

            if (necesidadActual == "Hambre")
            {
                if (hambreAd != null) hambreAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusEnojo != null)
                {
                    rufusSpriteRenderer.sprite = rufusEnojo;
                    GuardarSpriteGlobal("Hambre", rufusEnojo);
                }
            }
            else if (necesidadActual == "Caricias")
            {
                if (mimoAd != null) mimoAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusTriste != null)
                {
                    rufusSpriteRenderer.sprite = rufusTriste;
                    GuardarSpriteGlobal("Caricias", rufusTriste);
                }
            }
            else if (necesidadActual == "Dormir")
            {
                if (sueñoAd != null) sueñoAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusSueño != null)
                {
                    rufusSpriteRenderer.sprite = rufusSueño;
                    GuardarSpriteGlobal("Dormir", rufusSueño);
                }
            }
        }
        else
        {
            if (rufusSpriteRenderer != null && spriteNormal != null)
            {
                rufusSpriteRenderer.sprite = spriteNormal;
            }
        }
    }

    void GuardarSpriteGlobal(string necesidad, Sprite sprite)
    {
        if (ultimosSpritesPorNecesidad.ContainsKey(necesidad))
        {
            ultimosSpritesPorNecesidad[necesidad] = sprite;
        }
        else
        {
            ultimosSpritesPorNecesidad.Add(necesidad, sprite);
        }
    }

    public Sprite ObtenerUltimoSpriteGuardado(string necesidad)
    {
        if (ultimosSpritesPorNecesidad.ContainsKey(necesidad))
        {
            return ultimosSpritesPorNecesidad[necesidad];
        }
        return null;
    }

    public void MostrarSpriteTemporal(Sprite nuevoSprite, float duracion)
    {
        StopAllCoroutines();
        StartCoroutine(RutinaSpriteTemporario(nuevoSprite, duracion));
    }

    private IEnumerator RutinaSpriteTemporario(Sprite spriteTemp, float duracion)
    {
        bloquearActualizacion = true;

        if (rufusSpriteRenderer != null && spriteTemp != null)
        {
            rufusSpriteRenderer.sprite = spriteTemp;
        }

        if (hambreAd != null) hambreAd.SetActive(false);
        if (mimoAd != null) mimoAd.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        yield return new WaitForSeconds(duracion);

        bloquearActualizacion = false;
        ActualizarVisuales();
    }
}