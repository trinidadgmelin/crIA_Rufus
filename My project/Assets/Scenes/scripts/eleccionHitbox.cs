using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HitboxOtraEscena : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración")]
    public string nombreEscenaDelDialogo;

    [Header("Configuración de Fondo por Hover (Opcional)")]
    public SpriteRenderer fondoNido;
    public Sprite fondoHover;
    private Sprite fondoOriginal;

    void Start()
    {
        if (fondoNido != null)
        {
            fondoOriginal = fondoNido.sprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CambiarEscena();
    }

    public void CambiarEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscenaDelDialogo))
        {
            Debug.Log("Cambiando a la escena: " + nombreEscenaDelDialogo);
            SceneManager.LoadScene(nombreEscenaDelDialogo);
        }
        else
        {
            Debug.LogWarning("El nombre de la escena está vacío en el inspector.");
        }
    }

    // Métodos públicos para SelectorHitboxes y Teclado
    public void AbrirPorTeclado()
    {
        CambiarEscena();
    }

    public void ActivarFondoHover()
    {
        if (fondoNido != null && fondoHover != null)
        {
            fondoNido.sprite = fondoHover;
        }
    }

    public void DesactivarFondoHover()
    {
        if (fondoNido != null && fondoOriginal != null)
        {
            fondoNido.sprite = fondoOriginal;
        }
    }

    private void OnMouseEnter()
    {
        ActivarFondoHover();
    }

    private void OnMouseExit()
    {
        DesactivarFondoHover();
    }
}