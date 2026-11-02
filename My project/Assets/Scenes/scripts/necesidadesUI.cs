using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class necesidadesUI : MonoBehaviour
{
    public static necesidadesUI Instance;

    // Propiedad pública que indica si la UI está bloqueada (mostrando sprite temporal)
    public bool EstaBloqueado { get; private set; } = false;

    // ... (el resto de tu código)
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
        ReconectarReferencias();
        ActualizarVisuales();
    }

    void Start()
    {
        ReconectarReferencias();
        ActualizarVisuales();
    }

    void Update()
    {
        if (!bloquearActualizacion)
        {
            ActualizarVisuales();
        }
    }

    void ReconectarReferencias()
    {
        // Buscamos a Rufus (incluso si estuviera inactivo)
        if (rufusSpriteRenderer == null)
        {
            GameObject rufusObj = BuscarObjetoInactivo("Rufus");
            if (rufusObj != null) rufusSpriteRenderer = rufusObj.GetComponent<SpriteRenderer>();
        }

        // Buscamos los carteles permitiendo encontrarlos aunque empiecen apagados
        if (hambreAd == null) hambreAd = BuscarObjetoInactivo("hambreAd");
        if (mimoAd == null) mimoAd = BuscarObjetoInactivo("mimoAd");
        if (sueñoAd == null) sueñoAd = BuscarObjetoInactivo("sueñoAd");
    }

    // Método auxiliar para encontrar objetos en la escena actual aunque estén desactivados
    GameObject BuscarObjetoInactivo(string nombre)
    {
        // Usamos UnityEngine.Transform para evitar la ambigüedad con el RuleTile
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

        // Apagar todos los carteles por defecto
        if (hambreAd != null) hambreAd.SetActive(false);
        if (mimoAd != null) mimoAd.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        // Si hay necesidades en la cola, atendemos EXCLUSIVAMENTE a la primera de la fila
        if (elTimer.colaNecesidades.Count > 0)
        {
            string necesidadActual = elTimer.colaNecesidades[0];

            if (necesidadActual == "Hambre")
            {
                if (hambreAd != null) hambreAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusEnojo != null) rufusSpriteRenderer.sprite = rufusEnojo;
            }
            else if (necesidadActual == "Caricias")
            {
                if (mimoAd != null) mimoAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusTriste != null) rufusSpriteRenderer.sprite = rufusTriste;
            }
            else if (necesidadActual == "Dormir")
            {
                if (sueñoAd != null) sueñoAd.SetActive(true);
                if (rufusSpriteRenderer != null && rufusSueño != null) rufusSpriteRenderer.sprite = rufusSueño;
            }
        }
        else
        {
            // Si no hay ninguna necesidad pendiente, Rufus vuelve a su estado normal
            if (rufusSpriteRenderer != null && spriteNormal != null)
            {
                rufusSpriteRenderer.sprite = spriteNormal;
            }
        }
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