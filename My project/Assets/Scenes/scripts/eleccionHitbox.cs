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
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        CambiarEscena();
    }

    public void CambiarEscena()
    {
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

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
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        CambiarEscena();
    }

    public void ActivarFondoHover()
    {
        // 🎯 Si la UI está bloqueada (mostrando sprite temporal), ignoramos el hover por completo
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;

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
        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado) return;
        ActivarFondoHover();
    }

    private void OnMouseExit()
    {
        DesactivarFondoHover();
    }
}