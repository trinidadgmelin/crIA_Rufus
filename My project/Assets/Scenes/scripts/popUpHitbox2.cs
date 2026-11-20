using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class popUpHitbox2 : MonoBehaviour
{
    [Header("UI del Pop-up")]
    public GameObject popUp;

    [Header("Referencias de Textos e Imágenes ya creadas")]
    public TextMeshProUGUI texto1;
    public TextMeshProUGUI texto2;
    public Image imagen1;
    public Image imagen2;

    [Header("Guías Visuales")]
    public Image imagenGuia;          // Guía original (ej. Enter / Continuar / Salir)
    public Image imagenBarra;         // Nueva imagen de la barra

    [System.Serializable]
    public struct GrupoContenido
    {
        [TextArea(2, 5)] public string t1;
        [TextArea(2, 5)] public string t2;
        public Sprite img1;
        public Sprite img2;

        [Header("Guía Principal por Paso")]
        public Sprite guiaPaso0;      // Guía para el paso 0
        public Sprite guiaPaso1;      // Guía para el paso 1

        [Header("Nuevas Guías (Barra) por Paso")]
        public Sprite barra0;         // Barra para el paso 0
        public Sprite barra1;         // Barra para el paso 1
    }

    [Header("Grupos de Contenido (Exactamente 1)")]
    public GrupoContenido[] gruposDeContenido;

    private int pasoActual = 0;
    private bool popUpAbierto = false;
    private float tiempoPermitirAccion = 0f;
    private float tiempoPermitirReapertura = 0f;

    [Header("Configuración de Fondo por Hover")]
    public SpriteRenderer fondoNido;
    public Sprite fondoHover;
    private Sprite fondoOriginal;

    private Camera camara;

    void Start()
    {
        camara = Camera.main;

        if (popUp != null)
        {
            popUp.SetActive(false);
            if (imagenGuia == null)
            {
                Transform hijoGuia = popUp.transform.Find("guiaAvanzar");
                if (hijoGuia != null) imagenGuia = hijoGuia.GetComponent<Image>();
            }
        }
        else
        {
            Debug.LogWarning("No hay un pop-up asignado en: " + gameObject.name);
        }

        if (fondoNido != null)
        {
            fondoOriginal = fondoNido.sprite;
        }
    }

    void Update()
    {
        if (popUpAbierto)
        {
            if (Time.time < tiempoPermitirAccion) return;

            bool avanzarOAcabar = Input.GetKeyDown(KeyCode.A) ||
                               Input.GetKeyDown(KeyCode.Return) ||
                               Input.GetKeyDown(KeyCode.KeypadEnter) ||
                               (ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado);

            if (avanzarOAcabar)
            {
                if (pasoActual == 0)
                {
                    pasoActual = 1;
                    MostrarContenidoActual();
                    tiempoPermitirAccion = Time.time + 0.2f;
                }
                else
                {
                    CerrarPopup();
                }
            }
            return;
        }

        if (Time.time < tiempoPermitirReapertura) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (camara == null) return;

        Ray ray = camara.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                AbrirPopUpLogica();
                break;
            }
        }
    }

    void AbrirPopUpLogica()
    {
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

        if (popUp != null)
        {
            if (gruposDeContenido == null || gruposDeContenido.Length == 0) return;

            if (fondoNido != null && fondoOriginal != null)
            {
                fondoNido.sprite = fondoOriginal;
            }

            pasoActual = 0;
            MostrarContenidoActual();

            popUp.SetActive(true);
            popUpAbierto = true;
            timerNecesidades.HayPopupAbierto = true;

            tiempoPermitirAccion = Time.time + 0.3f;
        }
    }

    void MostrarContenidoActual()
    {
        if (gruposDeContenido == null || gruposDeContenido.Length == 0) return;

        GrupoContenido grupo = gruposDeContenido[0];

        if (pasoActual == 0)
        {
            if (texto1 != null) { texto1.gameObject.SetActive(true); texto1.text = grupo.t1; }
            if (texto2 != null) { texto2.gameObject.SetActive(false); }

            if (imagen1 != null) { imagen1.gameObject.SetActive(grupo.img1 != null); imagen1.sprite = grupo.img1; }
            if (imagen2 != null) { imagen2.gameObject.SetActive(false); }

            if (imagenGuia != null)
            {
                imagenGuia.gameObject.SetActive(grupo.guiaPaso0 != null);
                imagenGuia.sprite = grupo.guiaPaso0;
            }

            if (imagenBarra != null)
            {
                imagenBarra.gameObject.SetActive(grupo.barra0 != null);
                imagenBarra.sprite = grupo.barra0;
            }
        }
        else
        {
            if (texto1 != null) { texto1.gameObject.SetActive(false); }
            if (texto2 != null) { texto2.gameObject.SetActive(true); texto2.text = grupo.t2; }

            if (imagen1 != null) { imagen1.gameObject.SetActive(false); }
            if (imagen2 != null) { imagen2.gameObject.SetActive(grupo.img2 != null); imagen2.sprite = grupo.img2; }

            if (imagenGuia != null)
            {
                imagenGuia.gameObject.SetActive(grupo.guiaPaso1 != null);
                imagenGuia.sprite = grupo.guiaPaso1;
            }

            if (imagenBarra != null)
            {
                imagenBarra.gameObject.SetActive(grupo.barra1 != null);
                imagenBarra.sprite = grupo.barra1;
            }
        }
    }

    public void CerrarPopup()
    {
        if (popUp != null)
        {
            popUp.SetActive(false);
        }

        popUpAbierto = false;
        timerNecesidades.HayPopupAbierto = false;
        pasoActual = 0;
        tiempoPermitirReapertura = Time.time + 0.4f;

        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }

    private void OnMouseEnter()
    {
        if (popUpAbierto || Time.time < tiempoPermitirReapertura) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        ActivarFondoHover();
    }

    private void OnMouseExit()
    {
        DesactivarFondoHover();
    }

    public void AbrirPorTeclado()
    {
        if (popUpAbierto || Time.time < tiempoPermitirReapertura) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        AbrirPopUpLogica();
    }

    public void ActivarFondoHover()
    {
        if (popUpAbierto) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        if (fondoNido != null && fondoHover != null) fondoNido.sprite = fondoHover;
    }

    public void DesactivarFondoHover()
    {
        if (popUpAbierto) return;
        if (fondoNido != null && fondoOriginal != null) fondoNido.sprite = fondoOriginal;
    }
}