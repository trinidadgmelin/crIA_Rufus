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
                if (indicePasoActual < pasosDeEsteHitbox.Length - 1)
                {
                    indicePasoActual++;
                    MostrarPasoActual();
                }
                else
                {
                    CerrarPopup();
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
                    timerNecesidades.HayPopupAbierto = true; // <--- PAUSA LOS TIMERS
                }
                break;
            }
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
    }

    public void CerrarPopup()
    {
        if (popUp != null)
        {
            popUp.SetActive(false);
        }

        popUpAbierto = false;
        timerNecesidades.HayPopupAbierto = false; // <--- REACTIVA LOS TIMERS
        indicePasoActual = 0;

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