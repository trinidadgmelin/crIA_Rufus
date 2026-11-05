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

    [Header("Guía para Avanzar / Salir")]
    public TextMeshProUGUI guiaAvanzar;

    [System.Serializable]
    public struct GrupoContenido
    {
        [TextArea(2, 5)] public string t1;
        [TextArea(2, 5)] public string t2;
        public Sprite img1;
        public Sprite img2;
    }

    [Header("Grupos de Contenido (Exactamente 2)")]
    public GrupoContenido[] gruposDeContenido;

    private int pasoActual = 0; // 0 = Paso 1, 1 = Paso 2
    private bool popUpAbierto = false;
    private float tiempoPermitirAccion = 0f;     // Evita doble click al avanzar
    private float tiempoPermitirReapertura = 0f; // Evita que se reabra solo al cerrar

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
            if (guiaAvanzar == null)
            {
                Transform hijoGuia = popUp.transform.Find("guiaAvanzar");
                if (hijoGuia != null) guiaAvanzar = hijoGuia.GetComponent<TextMeshProUGUI>();
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
        // Si el pop-up está abierto, manejamos el avance o el cierre
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
                    tiempoPermitirAccion = Time.time + 0.2f; // Pequeña pausa para el siguiente paso
                }
                else
                {
                    CerrarPopup();
                }
            }
            return;
        }

        // Si está cerrado, evaluamos si podemos abrirlo
        if (Time.time < tiempoPermitirReapertura) return;

        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (camara == null)
            return;

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

            tiempoPermitirAccion = Time.time + 0.3f; // Bloqueo inicial al abrir
        }
    }

    void MostrarContenidoActual()
    {
        if (gruposDeContenido == null || gruposDeContenido.Length == 0) return;

        int index = Mathf.Clamp(pasoActual, 0, gruposDeContenido.Length - 1);
        GrupoContenido grupo = gruposDeContenido[index];

        if (pasoActual == 0)
        {
            if (texto1 != null) { texto1.gameObject.SetActive(true); texto1.text = grupo.t1; }
            if (texto2 != null) { texto2.gameObject.SetActive(false); }

            if (imagen1 != null) { imagen1.gameObject.SetActive(grupo.img1 != null); imagen1.sprite = grupo.img1; }
            if (imagen2 != null) { imagen2.gameObject.SetActive(false); }

            if (guiaAvanzar != null) guiaAvanzar.text = "ENTER PARA VER MÁS";
        }
        else
        {
            if (texto1 != null) { texto1.gameObject.SetActive(false); }
            if (texto2 != null) { texto2.gameObject.SetActive(true); texto2.text = grupo.t2; }

            if (imagen1 != null) { imagen1.gameObject.SetActive(false); }
            if (imagen2 != null) { imagen2.gameObject.SetActive(grupo.img2 != null); imagen2.sprite = grupo.img2; }

            if (guiaAvanzar != null) guiaAvanzar.text = "ENTER PARA SALIR";
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

        // PROTECCIÓN CLAVE: Bloqueamos la apertura por 0.4 segundos para que el botón no lo reabra solo
        tiempoPermitirReapertura = Time.time + 0.4f;

        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }

    private void OnMouseEnter()
    {
        if (popUpAbierto) return;
        if (Time.time < tiempoPermitirReapertura) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

        ActivarFondoHover();
    }

    private void OnMouseExit()
    {
        DesactivarFondoHover();
    }

    public void AbrirPorTeclado()
    {
        if (popUpAbierto) return;
        if (Time.time < tiempoPermitirReapertura) return;
        AbrirPopUpLogica();
    }

    public void ActivarFondoHover()
    {
        if (popUpAbierto) return;
        if (fondoNido != null && fondoHover != null)
        {
            fondoNido.sprite = fondoHover;
        }
    }

    public void DesactivarFondoHover()
    {
        if (popUpAbierto) return;
        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }
}