using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HitboxOtraEscena : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración")]
    public string nombreEscenaDelDialogo;

    // Este método se ejecuta automáticamente cuando el jugador hace clic sobre la hitbox en la UI
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
}