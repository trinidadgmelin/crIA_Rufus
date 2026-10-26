using UnityEngine;
using UnityEngine.SceneManagement;

public class HitboxOtraEscena : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscenaDelDialogo;

    void OnMouseDown()
    {
        IrAlDialogoConBotones();
    }

    public void IrAlDialogoConBotones()
    {
        dialogos.saltarABotones = true;

        if (!string.IsNullOrEmpty(nombreEscenaDelDialogo))
        {
            SceneManager.LoadScene(nombreEscenaDelDialogo);
            Debug.Log("Cambiando a la escena de diálogo y saltando a los botones...");
        }
        else
        {
            Debug.LogError("¡No escribiste el nombre de la escena en el Inspector!");
        }
    }
}