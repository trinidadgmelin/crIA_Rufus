using UnityEngine;
using UnityEngine.SceneManagement;

public class cargarEscena : MonoBehaviour
{
    // Esta función recibe el nombre de la escena y la carga
    public void CargarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
        Debug.Log("Cargando escena: " + nombreDeLaEscena);
    }
}