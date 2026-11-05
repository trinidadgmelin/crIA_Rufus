using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class popUpHitbox3 : MonoBehaviour
{
    [Header("UI del Pop-up")]
    public GameObject popUp;

    [Header("Componentes únicos en el Panel del Pop-up")]
    public TextMeshProUGUI textoUnico;
    public Image imagenUnica;

    [Header("Guía para Avanzar / Salir")]
    public TextMeshProUGUI guiaAvanzar;

    [System.Serializable]
    public struct PasoPopup
    {
        [TextArea(2, 5)] public string textoPaso;
        public Sprite imagenPaso;
    }

    [Header("Contenido Exclusivo de ESTE Hitbox")]
    public PasoPopup[] pasosDeEsteHitbox;

    [Header("Configuración de Fondo por Hover")]
    public SpriteRenderer fondoNido;
    public Sprite fondoHover;
    private Sprite fondoOriginal;

    private int indicePasoActual = 0;
    private bool popUpAbierto = false;
    private float tiempoPermitirAccion = 0f;     // Evita doble click al avanzar
    private float tiempoPermitirReapertura = 0f; // Evita que se reabra solo al cerrar
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
        if (popUpAbierto)
        {
            if (Time.time < tiempoPermitirAccion) return;

            bool avanzarOAcabar = Input.GetKeyDown(KeyCode.A) ||
                                 Input.GetKeyDown(KeyCode.Return) ||
                                 Input.GetKeyDown(KeyCode.KeypadEnter) ||
                                 (ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado);

            if (avanzarOAcabar)
            {
                if (indicePasoActual < pasosDeEsteHitbox.Length - 1)
                {
                    indicePasoActual++;
                    MostrarPasoActual();
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
            if (pasosDeEsteHitbox == null || pasosDeEsteHitbox.Length == 0)
            {
                Debug.LogWarning("No hay pasos configurados en este hitbox: " + gameObject.name);
                return;
            }

            if (fondoNido != null && fondoOriginal != null)
            {
                fondoNido.sprite = fondoOriginal;
            }

            indicePasoActual = 0;
            MostrarPasoActual();

            popUp.SetActive(true);
            popUpAbierto = true;
            timerNecesidades.HayPopupAbierto = true;

            tiempoPermitirAccion = Time.time + 0.3f;
        }
    }

    void MostrarPasoActual()
    {
        if (pasosDeEsteHitbox == null || pasosDeEsteHitbox.Length == 0) return;

        int index = Mathf.Clamp(indicePasoActual, 0, pasosDeEsteHitbox.Length - 1);
        PasoPopup pasoActual = pasosDeEsteHitbox[index];

        if (textoUnico != null)
        {
            textoUnico.gameObject.SetActive(true);
            textoUnico.text = pasoActual.textoPaso;
        }

        if (imagenUnica != null)
        {
            imagenUnica.gameObject.SetActive(pasoActual.imagenPaso != null);
            imagenUnica.sprite = pasoActual.imagenPaso;
        }

        if (guiaAvanzar != null)
        {
            if (indicePasoActual == pasosDeEsteHitbox.Length - 1)
            {
                guiaAvanzar.text = "ENTER PARA SALIR";
            }
            else
            {
                guiaAvanzar.text = "ENTER PARA VER MÁS";
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
        indicePasoActual = 0;

        // Protección contra auto-reapertura
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

    // Métodos públicos para SelectorHitboxes y Teclado
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