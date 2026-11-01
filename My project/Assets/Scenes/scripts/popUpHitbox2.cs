using UnityEngine;
using UnityEngine.UI;

public class popUpHitbox2 : MonoBehaviour
{
    [Header("UI del Pop-up")]
    public GameObject popUp;

    [Header("Configuración de Fondo por Hover")]
    public SpriteRenderer fondoNido;          // Arrastrá acá el objeto Image del fondo desde el Inspector
    public Sprite fondoHover;         // El sprite que querés mostrar cuando pasas el mouse por encima
    private Sprite fondoOriginal;     // Guarda el fondo original para restaurarlo al salir

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

        // Guardamos automáticamente el sprite que tiene el fondo al empezar
        if (fondoNido != null)
        {
            fondoOriginal = fondoNido.sprite;
        }
        else
        {
            Debug.LogWarning("Falta asignar el campo 'Fondo Image' en el Inspector para: " + gameObject.name);
        }
    }

    void Update()
    {
        // Lógica para hacer clic y abrir el pop-up
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
                    popUp.SetActive(true);

                break;
            }
        }
    }

    // Se ejecuta automáticamente cuando el mouse ENTRA en el Collider 2D
    private void OnMouseEnter()
    {
        if (fondoNido != null && fondoHover != null)
        {
            fondoNido.sprite = fondoHover;
        }
    }

    // Se ejecuta automáticamente cuando el mouse SALE del Collider 2D
    private void OnMouseExit()
    {
        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }
}