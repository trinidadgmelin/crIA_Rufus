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

    [System.Serializable]
    public struct GrupoContenido
    {
        [TextArea(2, 5)] public string t1;
        [TextArea(2, 5)] public string t2;
        public Sprite img1;
        public Sprite img2;
    }

    [Header("Grupos de Contenido (Se alternan al abrir, o por tecla A)")]
    public GrupoContenido[] gruposDeContenido;

    private int indiceGrupoActual = 0;
    private int pasoActual = 0;
    private bool popUpAbierto = false;

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
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (pasoActual == 0)
                {
                    pasoActual = 1;
                    MostrarContenidoActual();
                }
                else
                {
                    CerrarPopup(); // <--- Usamos el método unificado para cerrar y despausar
                }
            }
            return;
        }

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
                if (popUp != null)
                {
                    pasoActual = 0;
                    MostrarContenidoActual();

                    popUp.SetActive(true);
                    popUpAbierto = true;
                    timerNecesidades.HayPopupAbierto = true; // <--- PAUSA LOS TIMERS
                }
                break;
            }
        }
    }

    void MostrarContenidoActual()
    {
        if (gruposDeContenido == null || gruposDeContenido.Length == 0) return;

        GrupoContenido grupo = gruposDeContenido[indiceGrupoActual];

        if (pasoActual == 0)
        {
            if (texto1 != null) { texto1.gameObject.SetActive(true); texto1.text = grupo.t1; }
            if (texto2 != null) { texto2.gameObject.SetActive(false); }

            if (imagen1 != null) { imagen1.gameObject.SetActive(grupo.img1 != null); imagen1.sprite = grupo.img1; }
            if (imagen2 != null) { imagen2.gameObject.SetActive(false); }
        }
        else
        {
            if (texto1 != null) { texto1.gameObject.SetActive(false); }
            if (texto2 != null) { texto2.gameObject.SetActive(true); texto2.text = grupo.t2; }

            if (imagen1 != null) { imagen1.gameObject.SetActive(false); }
            if (imagen2 != null) { imagen2.gameObject.SetActive(grupo.img2 != null); imagen2.sprite = grupo.img2; }
        }
    }

    public void CerrarPopup()
    {
        if (popUp != null)
        {
            popUp.SetActive(false);
        }

        popUpAbierto = false;
        timerNecesidades.HayPopupAbierto = false; // <--- REACTIVA LOS TIMERS

        if (gruposDeContenido != null && gruposDeContenido.Length > 0)
        {
            indiceGrupoActual = (indiceGrupoActual + 1) % gruposDeContenido.Length;
        }

        pasoActual = 0;

        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }

    private void OnMouseEnter()
    {
        if (popUpAbierto) return;
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

        if (fondoNido != null && fondoHover != null)
        {
            fondoNido.sprite = fondoHover;
        }
    }

    private void OnMouseExit()
    {
        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }
}